# patient-care-management
Patient Care Management System for MCG take home.

This project implements a system to manage patients with the ability to:
- Create, update, retrieve, search and delete patient records.
- Upload, store, and associate clinical attachments (e.g., MRI, CAT Scan, doctor's reports) with patient records.
- Filter patients based on criteria such as name, medical condition, and attached document type.

Note: This project is not comprehensive and uses in-memory databases to keep things simple. Additionally, the unit tests are incomplete.

┌──────────────────────────────────────────────────────────────────────────────────┐
│                           Patient Management System                              │
├───────────────┐  ┌─────────────┐  ┌────────────────┐  ┌────────────────────────┐ │
│  Client Apps  │  │  API Layer  │  │ Service Layer  │  │     Data Layer         │ │
│               │  │             │  │                │  │                        │ │
│ ┌──────────┐  │  │┌───────────┐│  │ ┌────────────┐ │  │ ┌──────────────────┐   │ │
│ │Web Portal│ ◄┼──┼►Controllers│◄┼─┼►│Patient     │◄┼──┼►│Patient Repository│   │ │
│ └──────────┘  │  │└───────────┘│  │ │Service     │ │  │ └──────────────────┘   │ │
│               │  │             │  │ └────────────┘ │  │                        │ │
│ ┌──────────┐  │  │┌───────────┐│  │ ┌────────────┐ │  │ ┌──────────────────┐   │ │
│ │Mobile App│ ◄┼──┼►Auth       │◄┼─┼►│Med History │◄┼──┼►│Med History       │   │ │
│ └──────────┘  │  ││Middleware │◄┼─┼►│Service     │ │  │ │Repository        │   │ │
│               │  │└───────────┘│  │ └────────────┘ │  │ └──────────────────┘   │ │
│               │  │             │  │ ┌────────────┐ │  │ ┌──────────────────┐   │ │
│               │  │┌───────────┐│  │ │Auth        │◄┼──┼►│User/Role         │   │ │
│               │  ││Permission │◄┼─┼►│Service     │ │  │ │Repository        │   │ │
│               │  ││Filter     │◄┼─┼►└────────────┘ │  │ └──────────────────┘   │ │
│               │  │└───────────┘│  │                │  │                        │ │
└───────────────┘  └─────────────┘  └────────────────┘  └────────────────────────┘ │
                                                                                   │
┌──────────────────────────────────────────────────────────────────────────────────┐
│                          Cloud Infrastructure                                    │
│                                                                                  │
│  ┌──────────────┐    ┌────────────────┐    ┌────────────────┐    ┌────────────┐  │
│  │  App Service │    │ Azure Functions│    │  SQL Database  │    │ Blob       │  │
│  │  (Web API)   │    │ (Background    │    │  (Patient Data)│    │ Storage    │  │
│  │              │    │  Processing)   │    │                │    │ (Files)    │  │
│  └──────────────┘    └────────────────┘    └────────────────┘    └────────────┘  │
│                                                                                  │
│  ┌──────────────┐    ┌────────────────┐    ┌────────────────┐    ┌────────────┐  │
│  │ Azure AD B2C │    │ Search Index   │    │  Key Vault     │    │ App        │  │
│  │ (Identity)   │    │ (Azure Search) │    │  (Secrets)     │    │ Insights   │  │
│  └──────────────┘    └────────────────┘    └────────────────┘    └────────────┘  │
│                                                                                  │
└──────────────────────────────────────────────────────────────────────────────────┘
