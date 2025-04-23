using Microsoft.AspNetCore.Mvc;
using PatientCareManagement.Core.Models;
using PatientCareManagement.Core.Services;
using PatientCareManagement.Core.Enums;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace PatientCareManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly PatientService _patientService;

        public PatientsController(PatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAll()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> Get(Guid id)
        {
            var patient = await _patientService.GetPatientAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Patient>>> Search(
            [FromQuery] string searchTerm, 
            [FromQuery] string medicalCondition, 
            [FromQuery] AttachmentType? attachmentType)
        {
            var patients = await _patientService.SearchPatientsAsync(searchTerm, medicalCondition, attachmentType);
            return Ok(patients);
        }

        [HttpPost]
        public async Task<ActionResult<Patient>> Create([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdPatient = await _patientService.CreatePatientAsync(patient);
            return CreatedAtAction(nameof(Get), new { id = createdPatient.Id }, createdPatient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Patient patient)
        {
            if (id != patient.Id)
            {
                return BadRequest("Patient ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingPatient = await _patientService.GetPatientAsync(id);
            if (existingPatient == null)
            {
                return NotFound();
            }

            await _patientService.UpdatePatientAsync(patient);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var patient = await _patientService.GetPatientAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            await _patientService.DeletePatientAsync(id);
            return NoContent();
        }

        [HttpPost("{patientId}/medical-history")]
        public async Task<ActionResult<MedicalHistory>> AddMedicalHistory(Guid patientId, [FromBody] MedicalHistory medicalHistory)
        {
            var patient = await _patientService.GetPatientAsync(patientId);
            if (patient == null)
            {
                return NotFound();
            }

            var addedHistory = await _patientService.AddMedicalHistoryAsync(patientId, medicalHistory);
            return CreatedAtAction(nameof(Get), new { id = patientId }, addedHistory);
        }

        [HttpGet("medical-history/{medicalHistoryId}")]
        public async Task<ActionResult<MedicalHistory>> GetMedicalHistory(Guid medicalHistoryId)
        {
            var medicalHistory = await _patientService.GetMedicalHistoryWithAttachmentsAsync(medicalHistoryId);
            if (medicalHistory == null)
            {
                return NotFound();
            }
            return Ok(medicalHistory);
        }

        [HttpGet("medical-history/{medicalHistoryId}/attachments")]
        public async Task<ActionResult<IEnumerable<ClinicalAttachment>>> GetMedicalHistoryAttachments(Guid medicalHistoryId)
        {
            var attachments = await _patientService.GetAttachmentsByMedicalHistoryAsync(medicalHistoryId);
            return Ok(attachments);
        }

        [HttpPost("{patientId}/medical-history/{medicalHistoryId}/attachments")]
        public async Task<ActionResult<ClinicalAttachment>> UploadAttachmentToMedicalHistory(
            Guid patientId,
            Guid medicalHistoryId,
            IFormFile file,
            [FromForm] AttachmentType attachmentType,
            [FromForm] string notes)
        {
            var patient = await _patientService.GetPatientAsync(patientId);
            if (patient == null)
            {
                return NotFound("Patient not found");
            }

            // Verify medical history exists
            var medicalHistory = await _patientService.GetMedicalHistoryWithAttachmentsAsync(medicalHistoryId);
            if (medicalHistory == null)
            {
                return NotFound("Medical history not found");
            }

            if (medicalHistory.PatientId != patientId)
            {
                return BadRequest("Medical history does not belong to the specified patient");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded");
            }

            using (var stream = file.OpenReadStream())
            {
                var attachment = await _patientService.UploadAttachmentAsync(
                    patientId,
                    medicalHistoryId,
                    stream,
                    file.FileName,
                    file.ContentType,
                    attachmentType,
                    notes);

                return CreatedAtAction(
                    nameof(GetMedicalHistory), 
                    new { medicalHistoryId = medicalHistoryId }, 
                    attachment);
            }
        }

        [HttpPost("{patientId}/attachments")]
        public async Task<ActionResult<ClinicalAttachment>> UploadAttachment(
            Guid patientId,
            IFormFile file,
            [FromForm] AttachmentType attachmentType,
            [FromForm] string notes)
        {
            var patient = await _patientService.GetPatientAsync(patientId);
            if (patient == null)
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded");
            }

            using (var stream = file.OpenReadStream())
            {
                var attachment = await _patientService.UploadAttachmentAsync(
                    patientId,
                    null, // No specific medical history associated
                    stream,
                    file.FileName,
                    file.ContentType,
                    attachmentType,
                    notes);

                return CreatedAtAction(
                    nameof(Get), 
                    new { id = patientId }, 
                    attachment);
            }
        }

        [HttpGet("attachments/{attachmentId}")]
        public async Task<IActionResult> DownloadAttachment(Guid attachmentId)
        {
            var fileStream = await _patientService.DownloadAttachmentAsync(attachmentId);
            if (fileStream == null)
            {
                return NotFound();
            }

            // Would get the content type from the attachment metadata if implemented
            return File(fileStream, "application/octet-stream");
        }

        [HttpDelete("attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(Guid attachmentId)
        {
            await _patientService.DeleteAttachmentAsync(attachmentId);
            return NoContent();
        }
    }
}