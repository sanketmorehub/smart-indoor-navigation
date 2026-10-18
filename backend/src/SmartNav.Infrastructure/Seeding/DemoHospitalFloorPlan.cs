namespace SmartNav.Infrastructure.Seeding;

internal sealed record DemoFloorDefinition(int LevelIndex, string Code, string Name);

internal sealed record DemoServiceDefinition(
    string Name,
    string FloorCode,
    string CategoryCode,
    string Description,
    string Keywords);

internal static class DemoHospitalFloorPlan
{
    public static readonly DemoFloorDefinition[] Floors =
    [
        new(-2, "B2", "Basement 2 · Oncology & Nuclear Medicine"),
        new(-1, "B1", "Basement 1 · Imaging & Laboratories"),
        new(0, "G", "Ground Floor · Emergency & Patient Access"),
        new(1, "1", "First Floor · Specialist OPD & Rehabilitation"),
        new(2, "2", "Second Floor · Mother & Child"),
        new(3, "3", "Third Floor · Heart & Kidney Care"),
        new(4, "4", "Fourth Floor · Surgical Suites"),
        new(5, "5", "Fifth Floor · Neurology & Medical Critical Care"),
        new(6, "6", "Sixth Floor · General Inpatient Wards"),
        new(7, "7", "Seventh Floor · Private Inpatient Rooms"),
        new(8, "8", "Eighth Floor · Executive & International Care"),
        new(9, "9", "Ninth Floor · Mental Health & Administration"),
    ];

    public static readonly DemoServiceDefinition[] Services =
    [
        Service("Radiation Oncology", "B2", "DEPARTMENT", "Radiation treatment services; confirm clinical availability with hospital staff.", "cancer care radiotherapy radiation therapy oncology"),
        Service("Nuclear Medicine", "B2", "DEPARTMENT", "Nuclear medicine and isotope-based diagnostic services.", "PET SPECT nuclear scan isotope"),
        Service("PET-CT Scan", "B2", "LAB", "Combined PET and CT imaging service.", "pet ct scan oncology imaging"),
        Service("SPECT Scan", "B2", "LAB", "Single-photon emission computed tomography imaging.", "nuclear imaging spect"),
        Service("Iodine Therapy", "B2", "DEPARTMENT", "Iodine therapy service; follow the treating clinician's instructions.", "radioiodine thyroid treatment"),
        Service("Central Sterile Services (CSSD)", "B2", "DEPARTMENT", "Sterilization and processing service for hospital instruments.", "sterile supplies sterilization"),
        Service("Mortuary & Cold Storage", "B2", "DEPARTMENT", "Hospital mortuary and cold-storage service.", "mortuary deceased"),

        Service("Radiology & Imaging Center", "B1", "LAB", "Diagnostic imaging services; ask staff about preparation and appointments.", "radiology diagnostics imaging"),
        Service("MRI Scan", "B1", "LAB", "Magnetic resonance imaging service.", "magnetic resonance imaging mri"),
        Service("CT Scan", "B1", "LAB", "Computed tomography imaging service.", "computed tomography ct scan"),
        Service("Digital X-Ray", "B1", "LAB", "Digital radiography service.", "xray x ray radiograph"),
        Service("Ultrasound & Color Doppler", "B1", "LAB", "Ultrasound and color Doppler imaging.", "usg sonography doppler ultrasound"),
        Service("Mammography", "B1", "LAB", "Breast imaging and screening service.", "breast screening mammogram"),
        Service("BMD / DEXA Scan", "B1", "LAB", "Bone mineral density and DEXA scan service.", "bone density dexa osteoporosis"),
        Service("Pathology & Biochemistry Lab", "B1", "LAB", "Laboratory testing for blood, urine, hormones, vitamins, and other samples.", "blood urine laboratory tests pathology"),
        Service("Microbiology & Histopathology", "B1", "LAB", "Culture, biopsy, and tissue testing services.", "culture biopsy tumor histology"),
        Service("Blood Bank", "B1", "LAB", "Blood collection, cross-matching, and component storage service.", "blood donation transfusion plasma platelets"),
        Service("Hospital Kitchen", "B1", "DEPARTMENT", "Hospital food preparation and meal-service area.", "food meals kitchen"),
        Service("Laundry", "B1", "DEPARTMENT", "Hospital linen and laundry service.", "linen laundry"),

        Service("Ambulance Bay", "G", "EMERGENCY", "Emergency ambulance arrival and transfer point.", "ambulance entrance emergency"),
        Service("Triage & Resuscitation", "G", "EMERGENCY", "Emergency assessment and resuscitation area.", "triage emergency casualty trauma"),
        Service("Emergency Trauma Operating Theater", "G", "EMERGENCY", "Emergency trauma operating area.", "red zone trauma surgery emergency ot"),
        Service("Outpatient Department (OPD)", "G", "DEPARTMENT", "Outpatient registration, scheduling, and consultation services.", "outpatient appointment consultation opd"),
        Service("OPD Registration & Scheduling", "G", "RECEPTION", "Registration and appointment assistance for outpatient visits.", "token booking appointment registration"),
        Service("Billing & Discharge Desk", "G", "BILLING", "Billing settlement and discharge-document assistance.", "cashier payment settlement discharge"),
        Service("24/7 Pharmacy", "G", "PHARMACY", "In-house pharmacy for prescribed medicines and medical supplies.", "medical store chemist medicines medication pharmacy"),
        Service("TPA & Insurance Desk", "G", "BILLING", "Insurance approvals, cashless hospitalization, and corporate desk.", "insurance cashless tpa corporate"),
        Service("Health Check-up Lounge", "G", "DEPARTMENT", "Preventive, corporate, and pre-employment health-check packages.", "health checkup preventive screening executive"),
        Service("Helpdesk & Information", "G", "INFORMATION_DESK", "Visitor information and general assistance.", "help reception enquiry information"),

        Service("General & Internal Medicine", "1", "DEPARTMENT", "Adult primary care, routine illnesses, and chronic disease consultations.", "general medicine physician adult chronic care"),
        Service("Pulmonology", "1", "DEPARTMENT", "Consultations for lung and breathing conditions.", "respiratory asthma copd lung"),
        Service("Sleep Study Lab", "1", "LAB", "Sleep-disorder assessment and sleep-study service.", "sleep apnea sleep disorder"),
        Service("Orthopedics & Joint Replacement", "1", "DEPARTMENT", "Bone, fracture, spine, sports injury, and joint-care consultations.", "orthopaedics orthopedic bone fracture spine knee hip joint"),
        Service("Rheumatology", "1", "DEPARTMENT", "Consultations for joint and autoimmune conditions.", "arthritis autoimmune joint"),
        Service("Cast Room", "1", "ROOM", "Orthopedic casting and fracture support area.", "fracture plaster cast"),
        Service("Minor Procedure Room", "1", "ROOM", "Outpatient minor-procedure area.", "minor procedure clinic"),
        Service("Ophthalmology & Eye Clinic", "1", "DEPARTMENT", "Eye consultations, cataract, glaucoma, and vision care.", "ophthalmology eye cataract glaucoma vision"),
        Service("Vision Testing & Refraction", "1", "LAB", "Vision screening and refraction tests.", "eye test vision glasses refraction"),
        Service("Cataract & Glaucoma Clinic", "1", "DEPARTMENT", "Specialist cataract and glaucoma clinic.", "eye lens pressure"),
        Service("LASIK & Laser Eye Surgery", "1", "DEPARTMENT", "Laser eye procedure and assessment service.", "lasik laser vision surgery"),
        Service("ENT (Ear, Nose & Throat)", "1", "DEPARTMENT", "Ear, nose, throat, sinus, hearing, and vertigo consultations.", "ent otolaryngology sinus tonsil hearing vertigo"),
        Service("Audiometry & Hearing Test", "1", "LAB", "Hearing assessment and audiometry service.", "hearing test audiology"),
        Service("Dermatology & Cosmetology", "1", "DEPARTMENT", "Skin, hair, nail, and cosmetic dermatology consultations.", "skin hair nails cosmetic"),
        Service("Physiotherapy & Rehabilitation", "1", "DEPARTMENT", "Physical therapy and post-treatment rehabilitation.", "physical therapy rehab recovery"),
        Service("Sports Medicine & Rehabilitation", "1", "DEPARTMENT", "Sports injury assessment and rehabilitation.", "sports injury athlete"),
        Service("Hydrotherapy", "1", "DEPARTMENT", "Water-based rehabilitation service.", "water therapy"),
        Service("Occupational Therapy", "1", "DEPARTMENT", "Therapy supporting daily activities and functional recovery.", "occupational rehabilitation activities"),

        Service("Obstetrics & Gynecology (OB-GYN)", "2", "DEPARTMENT", "Women's health, pregnancy, and reproductive-health consultations.", "obgyn ob gyn pregnancy women maternity"),
        Service("Maternity & Antenatal Clinic", "2", "DEPARTMENT", "Prenatal and postnatal maternity consultations.", "antenatal prenatal postnatal pregnancy"),
        Service("Labor, Delivery & Recovery (LDR)", "2", "DEPARTMENT", "Labor, delivery, and recovery suites.", "labour labor delivery birth maternity"),
        Service("Pediatrics & Neonatology", "2", "DEPARTMENT", "Medical care for newborns, infants, children, and adolescents.", "pediatric paediatric child newborn infant"),
        Service("Vaccination Clinic", "2", "DEPARTMENT", "Childhood and other vaccination services.", "vaccine immunization vaccination"),
        Service("Neonatal ICU (NICU)", "2", "DEPARTMENT", "Critical care unit for premature or seriously ill newborns.", "newborn intensive incubator"),
        Service("Pediatric ICU (PICU)", "2", "DEPARTMENT", "Critical care unit for infants and children.", "children pediatric intensive care"),

        Service("Cardiology", "3", "DEPARTMENT", "Heart-disease consultations and cardiac care.", "heart cardiac heart failure pacemaker"),
        Service("Cardiothoracic Surgery", "3", "DEPARTMENT", "Cardiac and thoracic surgical consultations.", "heart surgery bypass cardiothoracic"),
        Service("Cardiac Cath Lab", "3", "LAB", "Interventional cardiac procedures including angiography and angioplasty.", "angiogram angioplasty stent cath"),
        Service("Cardiac Care Unit (CCU)", "3", "DEPARTMENT", "Continuous monitoring and intensive care for acute cardiac conditions.", "heart attack cardiac intensive"),
        Service("ECG & Echocardiogram (Echo)", "3", "LAB", "Non-invasive electrocardiogram and echocardiogram diagnostics.", "ecg echo heart test"),
        Service("Treadmill Test (TMT) & Holter", "3", "LAB", "Exercise stress testing and Holter monitoring.", "stress test cardiac holter"),
        Service("Dialysis Unit", "3", "DEPARTMENT", "Hemodialysis service for patients with kidney failure.", "hemodialysis kidney renal"),
        Service("Nephrology", "3", "DEPARTMENT", "Kidney-disease consultations and treatment.", "kidney nephrologist renal"),
        Service("Urology", "3", "DEPARTMENT", "Urinary tract, prostate, and kidney-stone consultations.", "urinary prostate bladder"),
        Service("Lithotripsy & Kidney Stone Care", "3", "DEPARTMENT", "Kidney-stone assessment and lithotripsy service.", "stone laser urinary"),

        Service("Operation Theaters (OT)", "4", "DEPARTMENT", "Modular operating theaters for scheduled surgical procedures.", "surgery operating room theater"),
        Service("Day Care Surgery Unit", "4", "DEPARTMENT", "Short-stay minor surgery and same-day discharge unit.", "day surgery short stay"),
        Service("Anesthesiology", "4", "DEPARTMENT", "Preoperative anesthesia consultation and perioperative care.", "anesthesia sedation pre surgery"),
        Service("Post-Anesthesia Care Unit (PACU)", "4", "DEPARTMENT", "Postoperative anesthesia recovery area.", "recovery post surgery"),
        Service("Surgical ICU (SICU)", "4", "DEPARTMENT", "Critical-care beds for patients after major surgery.", "surgical intensive postoperative"),

        Service("Medical ICU (MICU)", "5", "DEPARTMENT", "Critical-care beds for seriously ill medical patients.", "medical intensive critical care"),
        Service("Neurology & Neurosurgery", "5", "DEPARTMENT", "Brain, spine, nerve, stroke, tremor, and epilepsy consultations.", "neurology neurosurgery brain spine stroke epilepsy tremor"),
        Service("Stroke Clinic", "5", "DEPARTMENT", "Specialist stroke assessment and follow-up clinic.", "stroke cerebrovascular"),
        Service("EEG Diagnostics", "5", "LAB", "Electroencephalogram testing for brain electrical activity.", "eeg brain waves"),
        Service("EMG/NCV Diagnostics", "5", "LAB", "Electromyography and nerve-conduction testing.", "emg ncv nerve muscle"),
        Service("Gastroenterology", "5", "DEPARTMENT", "Digestive, liver, reflux, and bowel-condition consultations.", "gastro liver digestive acid reflux ibs"),
        Service("Endoscopy & Colonoscopy", "5", "LAB", "Endoscopy and colonoscopy diagnostic services.", "digestive scope bowel"),
        Service("ERCP Suite", "5", "LAB", "Endoscopic retrograde cholangiopancreatography suite.", "ercp bile duct pancreas"),
        Service("Endocrinology & Diabetology", "5", "DEPARTMENT", "Diabetes, thyroid, and hormonal-condition consultations.", "diabetes thyroid hormone endocrine"),

        Service("Inpatient Department (IPD)", "6", "DEPARTMENT", "Inpatient admission and general ward services.", "inpatient admission ward"),
        Service("General Wards", "6", "ROOM", "General inpatient ward rooms.", "general ward patient rooms"),
        Service("Semi-Private Wards", "6", "ROOM", "Semi-private inpatient accommodation.", "semi private shared room"),
        Service("Nursing Station · Floor 6", "6", "INFORMATION_DESK", "Nursing assistance point for this ward floor.", "nurse ward help"),
        Service("Doctor-on-Duty Room · Floor 6", "6", "OFFICE", "On-duty medical staff room.", "doctor resident physician"),
        Service("Clinical Pharmacy · Floor 6", "6", "PHARMACY", "Satellite pharmacy service for admitted patients.", "inpatient medication medicines"),
        Service("Dietetics & Nutrition", "6", "DEPARTMENT", "Patient diet planning and nutrition consultations.", "diet diabetic low sodium nutrition"),

        Service("Private Single Rooms", "7", "ROOM", "Private single-room inpatient accommodation.", "private room inpatient"),
        Service("Nursing Station · Floor 7", "7", "INFORMATION_DESK", "Nursing assistance point for this ward floor.", "nurse ward help"),
        Service("Doctor-on-Duty Room · Floor 7", "7", "OFFICE", "On-duty medical staff room.", "doctor resident physician"),
        Service("Clinical Pharmacy · Floor 7", "7", "PHARMACY", "Satellite pharmacy service for admitted patients.", "inpatient medication medicines"),

        Service("Deluxe & Executive Suites", "8", "ROOM", "Premium inpatient rooms and executive suites.", "deluxe suite executive private"),
        Service("International Patient Services", "8", "INFORMATION_DESK", "Visitor coordination for international patients.", "international visa travel foreign patient"),
        Service("Language Translation & Travel Desk", "8", "INFORMATION_DESK", "Language assistance, visa guidance, and travel coordination.", "interpreter translation visa travel"),
        Service("Nursing Station · Floor 8", "8", "INFORMATION_DESK", "Nursing assistance point for this inpatient floor.", "nurse ward help"),

        Service("Psychiatry & Behavioral Health", "9", "DEPARTMENT", "Mental-health consultations and behavioral-health support.", "psychiatry mental health counseling depression anxiety"),
        Service("Counseling & Addiction Therapy", "9", "DEPARTMENT", "Counseling and addiction-recovery therapy services.", "therapy addiction substance support"),
        Service("Hospital Administration", "9", "ADMINISTRATION", "Hospital administrative offices; visitor access may be restricted.", "administration management office"),
        Service("Medical Records Department (MRD)", "9", "ADMINISTRATION", "Medical records and document-request services.", "records history discharge summary registration"),
        Service("Human Resources (HR)", "9", "OFFICE", "Hospital human-resources office.", "staff employment hr"),
        Service("Auditorium & Board Rooms", "9", "CONFERENCE_ROOM", "Hospital seminar, training, and meeting spaces.", "conference seminar meeting lecture"),
        Service("Doctors' Cafeteria & Lounge", "9", "CAFE", "Staff dining and lounge area; access may be restricted.", "food cafeteria doctor lounge"),
    ];

    private static DemoServiceDefinition Service(
        string name,
        string floorCode,
        string categoryCode,
        string description,
        string keywords) =>
        new(name, floorCode, categoryCode, description, keywords);
}
