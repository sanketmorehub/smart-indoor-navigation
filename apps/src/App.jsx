import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { findRoute, loadBuilding, resolveQrToken } from "./api";
import "./App.css";

const translations = {
  en: {
    "Main navigation": "Main navigation", "Find a location": "Find a location", "Floor map": "Floor map",
    Directions: "Directions", "Visitor navigation": "Visitor navigation", "Choose language": "Choose language",
    "Help assistant": "Help assistant", "Find your way.": "Find your way.", "Feel taken care of.": "Feel taken care of.",
    "Start with directions": "Start with directions", "PLAN YOUR VISIT": "PLAN YOUR VISIT",
    "INDOOR WAYFINDING": "INDOOR WAYFINDING",
    "Welcome to Aura Hospital. Scan any QR code to find your way.": "Welcome to Aura Hospital. Scan any QR code to find your way.",
    "Where do you need to go?": "Where do you need to go?", "Where are you now?": "Where are you now?",
    "Your current location": "Your current location", "Scan a location QR code": "Scan a location QR code",
    "Starting at": "Starting at", "Choose your destination": "Choose your destination",
    "Search departments and services": "Search departments and services",
    "Try Cardiology or Pharmacy": "Try Cardiology or Pharmacy", "No locations match": "No locations match",
    "Step-free route": "Step-free route", "Avoid stairs and prefer lifts": "Avoid stairs and prefer lifts",
    "Finding the best route…": "Finding the best route…", "Show my route": "Show my route",
    "LIVE ROUTE PREVIEW": "LIVE ROUTE PREVIEW", "Choose a floor to explore hospital locations": "Choose a floor to explore hospital locations",
    "Published floor graph": "Published floor graph", "Select a floor": "Select a floor",
    "Your directions will appear here": "Your directions will appear here",
    "Select your starting location and a destination, then show your route.": "Select your starting location and a destination, then show your route.",
    "Route calculated": "Route calculated", DISTANCE: "DISTANCE", "WALKING TIME": "WALKING TIME",
    ROUTE: "ROUTE", "Fastest available": "Fastest available", "Visitor guidance:": "Visitor guidance:",
    "Helping every visitor find their way.": "Helping every visitor find their way.",
    "Ask Aura": "Ask Aura", "Aura Help Assistant": "Aura Help Assistant",
    "I can help you find departments, use QR checkpoints, and understand your route.": "I can help you find departments, use QR checkpoints, and understand your route.",
    "How do I get directions?": "How do I get directions?", "How do I scan a QR code?": "How do I scan a QR code?",
    "Where am I?": "Where am I?", "Ask a question…": "Ask a question…", Send: "Send",
    "Choose a starting point, search for a department, select it, and tap Show my route.": "Choose a starting point, search for a department, select it, and tap Show my route.",
    "Tap Scan a location QR code and allow camera access, or paste the QR link manually.": "Tap Scan a location QR code and allow camera access, or paste the QR link manually.",
    "Your current starting point is": "Your current starting point is",
    "I can help with directions, QR scanning, step-free routes, or finding a department.": "I can help with directions, QR scanning, step-free routes, or finding a department.",
    "Take {connector} from {from} to {to}.": "Take {connector} from {from} to {to}.",
    "Take {connector} to {to}.": "Take {connector} to {to}.",
    "Follow the corridor for {distance} m towards {target}.": "Follow the corridor for {distance} m towards {target}.",
    "You have arrived at {destination}.": "You have arrived at {destination}.",
    "Directions to {destination}": "Directions to {destination}",
    "From {start} ({floor}) to {destination} · {destinationFloor}": "From {start} ({floor}) to {destination} · {destinationFloor}",
    "Lift A": "Lift A", "Stairs": "stairs",
    "About {minutes} min": "About {minutes} min",
    "Use my phone location": "Use my phone location", "Stop location tracking": "Stop location tracking",
    "Live phone location": "Live phone location", "Location accuracy": "Location accuracy",
    "GPS helps outdoors. Indoors it can drift; routes still start from your selected QR checkpoint.": "GPS helps outdoors. Indoors it can drift; routes still start from your selected QR checkpoint.",
    "Location permission requires a secure HTTPS page. Open the secure Aura link or use a QR checkpoint.": "Location permission requires a secure HTTPS page. Open the secure Aura link or use a QR checkpoint.",
    "Location permission was denied. Allow location access in your browser settings and try again.": "Location permission was denied. Allow location access in your browser settings and try again.",
    "Could not get your location. Move near a window or try again.": "Could not get your location. Move near a window or try again.",
    "Waiting for GPS…": "Waiting for GPS…", "GPS unavailable": "GPS unavailable",
    "Open current location in Google Maps": "Open current location in Google Maps",
    "Plan a route to this location": "Plan a route to this location",
    "Basement 2 · Oncology & Nuclear Medicine": "Basement 2 · Oncology & Nuclear Medicine",
    "Basement 1 · Imaging & Laboratories": "Basement 1 · Imaging & Laboratories",
    "Ground Floor · Emergency & Patient Access": "Ground Floor · Emergency & Patient Access",
    "First Floor · Specialist OPD & Rehabilitation": "First Floor · Specialist OPD & Rehabilitation",
    "Second Floor · Mother & Child": "Second Floor · Mother & Child",
    "Third Floor · Heart & Kidney Care": "Third Floor · Heart & Kidney Care",
    "Fourth Floor · Surgical Suites": "Fourth Floor · Surgical Suites",
    "Fifth Floor · Neurology & Medical Critical Care": "Fifth Floor · Neurology & Medical Critical Care",
    "Sixth Floor · General Inpatient Wards": "Sixth Floor · General Inpatient Wards",
    "Seventh Floor · Private Inpatient Rooms": "Seventh Floor · Private Inpatient Rooms",
    "Eighth Floor · Executive & International Care": "Eighth Floor · Executive & International Care",
    "Ninth Floor · Mental Health & Administration": "Ninth Floor · Mental Health & Administration",
    "Illustrative demo map · verify on-site": "Illustrative demo map · verify on-site",
    "STEP 3 · FOLLOW YOUR JOURNEY": "STEP 3 · FOLLOW YOUR JOURNEY",
    "Routes use the illustrative demo layout. Confirm directions with hospital staff.": "Routes use the illustrative demo layout. Confirm directions with hospital staff.",
    "Indoor routes are based on the building’s published map. QR checkpoints set your current location; your browser does not track your position between scans. For emergencies, contact hospital staff.": "Indoor routes are based on the illustrative map. QR checkpoints set your starting point; GPS may drift inside. For emergencies, contact hospital staff.",
    "From": "From", "to": "to", "your starting point": "your starting point", "current floor": "current floor",
    "Navigation help · Available here": "Navigation help · Available here",
    "Close help assistant": "Close help assistant",
    "This built-in assistant uses local navigation help and does not send chats to an external AI service.": "This built-in assistant uses local navigation help and does not send chats to an external AI service.",
    "No published floor points are available.": "No published floor points are available.",
    "The demo floor plan is illustrative and must be verified on site.": "The demo floor plan is illustrative and must be verified on site.",
    "the next corridor junction": "the next corridor junction",
    "the next waypoint": "the next waypoint",
    "Your location": "Your location", Destination: "Destination", Route: "Route",
    "Recommended route": "Recommended route", "map points": "map points",
    "Hospital floor map": "Hospital floor map", "Remove destination": "Remove destination",
    "Matching destinations": "Matching destinations", "Clear destination search": "Clear destination search",
    "No active location checkpoints": "No active location checkpoints", Floor: "Floor",
    Reception: "Reception", Cardiology: "Cardiology",
    Department: "Department", Lab: "Lab", Emergency: "Emergency", Billing: "Billing",
    Pharmacy: "Pharmacy", Room: "Room", "Information Desk": "Information Desk",
    Administration: "Administration", Entrance: "Entrance", Exit: "Exit",
    Cafe: "Cafe", "Conference Room": "Conference Room", "Medical Store": "Medical Store", Office: "Office",
    "Map north direction": "Map north direction",
    "Connecting to hospital navigation…": "Connecting to hospital navigation…",
    "Navigation is temporarily unavailable": "Navigation is temporarily unavailable",
    "Try again": "Try again", "Check that the API and database are running, then retry.": "Check that the API and database are running, then retry.",
    "Choose a starting location and a destination to calculate your route.": "Choose a starting location and a destination to calculate your route.",
    "Show {floor} map": "Show {floor} map",
    "✓ Real shortest-path directions": "✓ Real shortest-path directions",
    "✓ QR location checkpoints": "✓ QR location checkpoints",
    "✓ Step-free route option": "✓ Step-free route option",
  },
  mr: {
    "Main navigation": "मुख्य नेव्हिगेशन", "Find a location": "ठिकाण शोधा", "Floor map": "मजल्याचा नकाशा",
    Directions: "मार्गदर्शन", "Visitor navigation": "अभ्यागत नेव्हिगेशन", "Choose language": "भाषा निवडा",
    "Help assistant": "मदत सहाय्यक", "Find your way.": "तुमचा मार्ग शोधा.", "Feel taken care of.": "आमची काळजी अनुभवा.",
    "Start with directions": "मार्गदर्शन सुरू करा", "PLAN YOUR VISIT": "भेटीचे नियोजन करा",
    "INDOOR WAYFINDING": "रुग्णालयातील मार्गदर्शन",
    "Welcome to Aura Hospital. Scan any QR code to find your way.": "Aura Hospital मध्ये आपले स्वागत आहे. मार्ग शोधण्यासाठी कोणताही QR कोड स्कॅन करा.",
    "Where do you need to go?": "तुम्हाला कुठे जायचे आहे?", "Where are you now?": "तुम्ही सध्या कुठे आहात?",
    "Your current location": "तुमचे सध्याचे ठिकाण", "Scan a location QR code": "ठिकाणाचा QR कोड स्कॅन करा",
    "Starting at": "सुरुवात", "Choose your destination": "तुमचे गंतव्य निवडा",
    "Search departments and services": "विभाग आणि सेवा शोधा",
    "Try Cardiology or Pharmacy": "कार्डिओलॉजी किंवा फार्मसी शोधा", "No locations match": "या शोधाशी जुळणारे ठिकाण नाही",
    "Step-free route": "पायऱ्यांशिवाय मार्ग", "Avoid stairs and prefer lifts": "पायऱ्या टाळा आणि लिफ्ट वापरा",
    "Finding the best route…": "योग्य मार्ग शोधत आहे…", "Show my route": "माझा मार्ग दाखवा",
    "LIVE ROUTE PREVIEW": "थेट मार्ग पूर्वावलोकन", "Choose a floor to explore hospital locations": "ठिकाणे पाहण्यासाठी मजला निवडा",
    "Published floor graph": "प्रकाशित मजला नकाशा", "Select a floor": "मजला निवडा",
    "Your directions will appear here": "तुमचे मार्गदर्शन येथे दिसेल",
    "Select your starting location and a destination, then show your route.": "सुरुवातीचे ठिकाण आणि गंतव्य निवडा, नंतर मार्ग दाखवा.",
    "Route calculated": "मार्ग तयार आहे", DISTANCE: "अंतर", "WALKING TIME": "चालण्याचा वेळ",
    ROUTE: "मार्ग", "Fastest available": "उपलब्ध जलद मार्ग", "Visitor guidance:": "अभ्यागतांसाठी सूचना:",
    "Helping every visitor find their way.": "प्रत्येक अभ्यागताला मार्ग शोधण्यात मदत.",
    "Ask Aura": "Aura ला विचारा", "Aura Help Assistant": "Aura मदत सहाय्यक",
    "I can help you find departments, use QR checkpoints, and understand your route.": "विभाग शोधणे, QR कोड वापरणे आणि मार्ग समजून घेण्यासाठी मी मदत करू शकतो.",
    "How do I get directions?": "मार्गदर्शन कसे मिळवू?", "How do I scan a QR code?": "QR कोड कसा स्कॅन करू?",
    "Where am I?": "मी कुठे आहे?", "Ask a question…": "प्रश्न विचारा…", Send: "पाठवा",
    "Choose a starting point, search for a department, select it, and tap Show my route.": "सुरुवातीचे ठिकाण निवडा, विभाग शोधा, तो निवडा आणि 'माझा मार्ग दाखवा' दाबा.",
    "Tap Scan a location QR code and allow camera access, or paste the QR link manually.": "ठिकाणाचा QR कोड स्कॅन करा आणि कॅमेरा परवानगी द्या किंवा QR लिंक येथे पेस्ट करा.",
    "Your current starting point is": "तुमचे सध्याचे सुरुवातीचे ठिकाण",
    "I can help with directions, QR scanning, step-free routes, or finding a department.": "मार्गदर्शन, QR स्कॅन, पायऱ्यांशिवाय मार्ग किंवा विभाग शोधण्यात मी मदत करू शकतो.",
    "Take {connector} from {from} to {to}.": "{from} येथून {to} पर्यंत {connector} घ्या.",
    "Take {connector} to {to}.": "{to} पर्यंत {connector} घ्या.",
    "Follow the corridor for {distance} m towards {target}.": "कॉरिडॉरमध्ये {distance} मीटर पुढे चला. पुढील ठिकाण: {target}.",
    "You have arrived at {destination}.": "तुम्ही {destination} येथे पोहोचलात.",
    "Directions to {destination}": "{destination} पर्यंतचा मार्ग",
    "From {start} ({floor}) to {destination} · {destinationFloor}": "सुरुवात: {start} ({floor}) · गंतव्य: {destination} · {destinationFloor}",
    "Lift A": "लिफ्ट A", "Stairs": "जिना",
    "About {minutes} min": "सुमारे {minutes} मिनिटे",
    "Use my phone location": "माझे फोनचे ठिकाण वापरा", "Stop location tracking": "ठिकाण ट्रॅक करणे थांबवा",
    "Live phone location": "फोनचे थेट ठिकाण", "Location accuracy": "ठिकाणाची अचूकता",
    "GPS helps outdoors. Indoors it can drift; routes still start from your selected QR checkpoint.": "बाहेर GPS मदत करते. इमारतीत अचूकता कमी असू शकते; मार्ग निवडलेल्या QR ठिकाणापासून सुरू होतो.",
    "Location permission requires a secure HTTPS page. Open the secure Aura link or use a QR checkpoint.": "ठिकाणाची परवानगी HTTPS पृष्ठावरच मिळते. सुरक्षित Aura लिंक उघडा किंवा QR ठिकाण वापरा.",
    "Location permission was denied. Allow location access in your browser settings and try again.": "ठिकाणाची परवानगी नाकारली. ब्राउझर सेटिंगमध्ये परवानगी द्या आणि पुन्हा प्रयत्न करा.",
    "Could not get your location. Move near a window or try again.": "तुमचे ठिकाण मिळाले नाही. खिडकीजवळ जा किंवा पुन्हा प्रयत्न करा.",
    "Waiting for GPS…": "GPS ची वाट पाहत आहे…", "GPS unavailable": "GPS उपलब्ध नाही",
    "Open current location in Google Maps": "सध्याचे ठिकाण Google Maps मध्ये उघडा",
    "Plan a route to this location": "या ठिकाणासाठी मार्ग तयार करा",
    "Basement 2 · Oncology & Nuclear Medicine": "तळघर २ · कर्करोग व न्यूक्लियर मेडिसिन",
    "Basement 1 · Imaging & Laboratories": "तळघर १ · इमेजिंग व प्रयोगशाळा",
    "Ground Floor · Emergency & Patient Access": "तळमजला · आपत्कालीन सेवा व रुग्ण प्रवेश",
    "First Floor · Specialist OPD & Rehabilitation": "पहिला मजला · विशेषज्ञ ओपीडी व पुनर्वसन",
    "Second Floor · Mother & Child": "दुसरा मजला · माता व बालसेवा",
    "Third Floor · Heart & Kidney Care": "तिसरा मजला · हृदय व मूत्रपिंड सेवा",
    "Fourth Floor · Surgical Suites": "चौथा मजला · शस्त्रक्रिया विभाग",
    "Fifth Floor · Neurology & Medical Critical Care": "पाचवा मजला · न्यूरोलॉजी व वैद्यकीय अतिदक्षता",
    "Sixth Floor · General Inpatient Wards": "सहावा मजला · सामान्य रुग्ण कक्ष",
    "Seventh Floor · Private Inpatient Rooms": "सातवा मजला · खासगी रुग्ण खोल्या",
    "Eighth Floor · Executive & International Care": "आठवा मजला · एक्झिक्युटिव्ह व आंतरराष्ट्रीय सेवा",
    "Ninth Floor · Mental Health & Administration": "नववा मजला · मानसिक आरोग्य व प्रशासन",
    "Illustrative demo map · verify on-site": "प्रात्यक्षिक नकाशा · प्रत्यक्ष ठिकाणी पडताळा",
    "STEP 3 · FOLLOW YOUR JOURNEY": "टप्पा ३ · तुमच्या मार्गाचे अनुसरण करा",
    "Routes use the illustrative demo layout. Confirm directions with hospital staff.": "मार्ग प्रात्यक्षिक नकाशावर आधारित आहेत. रुग्णालयातील कर्मचाऱ्यांकडून दिशा पडताळा.",
    "Indoor routes are based on the building’s published map. QR checkpoints set your current location; your browser does not track your position between scans. For emergencies, contact hospital staff.": "आतील मार्ग प्रात्यक्षिक नकाशावर आधारित आहेत. QR कोड सुरुवातीचे ठिकाण ठरवतो; इमारतीत GPS अचूक नसेल. आपत्कालीन स्थितीत रुग्णालय कर्मचाऱ्यांशी संपर्क साधा.",
    "From": "पासून", "to": "पर्यंत", "your starting point": "तुमचे सुरुवातीचे ठिकाण", "current floor": "सध्याचा मजला",
    "Navigation help · Available here": "नेव्हिगेशन मदत · येथे उपलब्ध",
    "Close help assistant": "मदत बंद करा",
    "This built-in assistant uses local navigation help and does not send chats to an external AI service.": "हा सहाय्यक स्थानिक मदत वापरतो; तुमचे संदेश बाह्य AI सेवेकडे पाठवत नाही.",
    "No published floor points are available.": "या मजल्यासाठी नकाशावरील ठिकाणे उपलब्ध नाहीत.",
    "The demo floor plan is illustrative and must be verified on site.": "हा प्रात्यक्षिक मजला नकाशा आहे; प्रत्यक्ष ठिकाणी पडताळणी आवश्यक आहे.",
    "the next corridor junction": "पुढील कॉरिडॉर चौक",
    "the next waypoint": "पुढील मार्गबिंदू",
    "Your location": "तुमचे ठिकाण", Destination: "गंतव्य", Route: "मार्ग",
    "Recommended route": "सुचवलेला मार्ग", "map points": "नकाशावरील ठिकाणे",
    "Hospital floor map": "रुग्णालयाचा मजला नकाशा", "Remove destination": "गंतव्य काढा",
    "Matching destinations": "जुळणारी ठिकाणे", "Clear destination search": "गंतव्य शोध पुसा",
    "No active location checkpoints": "सध्या कोणतेही ठिकाण तपासणी-बिंदू उपलब्ध नाहीत", Floor: "मजला",
    Reception: "स्वागत कक्ष", Cardiology: "हृदयरोग विभाग",
    Department: "विभाग", Lab: "प्रयोगशाळा", Emergency: "आपत्कालीन सेवा", Billing: "बिलिंग",
    Pharmacy: "औषधालय", Room: "खोली", "Information Desk": "माहिती कक्ष",
    Administration: "प्रशासन", Entrance: "प्रवेशद्वार", Exit: "निर्गमन",
    Cafe: "कॅफे", "Conference Room": "परिषद कक्ष", "Medical Store": "औषध दुकान", Office: "कार्यालय",
    "Map north direction": "नकाशाची उत्तर दिशा",
    "Connecting to hospital navigation…": "रुग्णालय नेव्हिगेशनशी जोडत आहे…",
    "Navigation is temporarily unavailable": "नेव्हिगेशन सध्या उपलब्ध नाही",
    "Try again": "पुन्हा प्रयत्न करा", "Check that the API and database are running, then retry.": "API आणि डेटाबेस सुरू आहेत याची खात्री करून पुन्हा प्रयत्न करा.",
    "Choose a starting location and a destination to calculate your route.": "मार्ग काढण्यासाठी सुरुवातीचे ठिकाण आणि गंतव्य निवडा.",
    "Show {floor} map": "{floor} नकाशा दाखवा",
    "✓ Real shortest-path directions": "✓ अचूक मार्गदर्शन",
    "✓ QR location checkpoints": "✓ QR ठिकाण तपासणी",
    "✓ Step-free route option": "✓ पायऱ्यांशिवाय मार्ग",
  },
  hi: {
    "Main navigation": "मुख्य नेविगेशन", "Find a location": "जगह खोजें", "Floor map": "मंज़िल का नक्शा",
    Directions: "दिशा-निर्देश", "Visitor navigation": "आगंतुक नेविगेशन", "Choose language": "भाषा चुनें",
    "Help assistant": "सहायता सहायक", "Find your way.": "अपना रास्ता खोजें।", "Feel taken care of.": "देखभाल का भरोसा पाएँ।",
    "Start with directions": "दिशा-निर्देश शुरू करें", "PLAN YOUR VISIT": "अपनी यात्रा की योजना बनाएँ",
    "INDOOR WAYFINDING": "अस्पताल में रास्ता खोजें",
    "Welcome to Aura Hospital. Scan any QR code to find your way.": "Aura Hospital में आपका स्वागत है। रास्ता जानने के लिए कोई भी QR कोड स्कैन करें।",
    "Where do you need to go?": "आपको कहाँ जाना है?", "Where are you now?": "आप अभी कहाँ हैं?",
    "Your current location": "आपका वर्तमान स्थान", "Scan a location QR code": "स्थान का QR कोड स्कैन करें",
    "Starting at": "शुरुआत", "Choose your destination": "अपना गंतव्य चुनें",
    "Search departments and services": "विभाग और सेवाएँ खोजें",
    "Try Cardiology or Pharmacy": "कार्डियोलॉजी या फार्मेसी खोजें", "No locations match": "कोई स्थान मेल नहीं खाता",
    "Step-free route": "सीढ़ी-मुक्त रास्ता", "Avoid stairs and prefer lifts": "सीढ़ियों से बचें और लिफ्ट चुनें",
    "Finding the best route…": "सबसे अच्छा रास्ता खोज रहे हैं…", "Show my route": "मेरा रास्ता दिखाएँ",
    "LIVE ROUTE PREVIEW": "लाइव रास्ता पूर्वावलोकन", "Choose a floor to explore hospital locations": "स्थान देखने के लिए मंज़िल चुनें",
    "Published floor graph": "प्रकाशित मंज़िल नक्शा", "Select a floor": "मंज़िल चुनें",
    "Your directions will appear here": "आपके दिशा-निर्देश यहाँ दिखेंगे",
    "Select your starting location and a destination, then show your route.": "शुरुआती स्थान और गंतव्य चुनें, फिर रास्ता दिखाएँ।",
    "Route calculated": "रास्ता तैयार है", DISTANCE: "दूरी", "WALKING TIME": "चलने का समय",
    ROUTE: "रास्ता", "Fastest available": "उपलब्ध सबसे तेज़", "Visitor guidance:": "आगंतुक सूचना:",
    "Helping every visitor find their way.": "हर आगंतुक को रास्ता खोजने में मदद।",
    "Ask Aura": "Aura से पूछें", "Aura Help Assistant": "Aura सहायता सहायक",
    "I can help you find departments, use QR checkpoints, and understand your route.": "विभाग खोजने, QR कोड इस्तेमाल करने और रास्ता समझने में मैं मदद कर सकता हूँ।",
    "How do I get directions?": "रास्ता कैसे मिलेगा?", "How do I scan a QR code?": "QR कोड कैसे स्कैन करें?",
    "Where am I?": "मैं कहाँ हूँ?", "Ask a question…": "अपना सवाल पूछें…", Send: "भेजें",
    "Choose a starting point, search for a department, select it, and tap Show my route.": "शुरुआती जगह चुनें, विभाग खोजें, उसे चुनें और 'मेरा रास्ता दिखाएँ' दबाएँ।",
    "Tap Scan a location QR code and allow camera access, or paste the QR link manually.": "स्थान का QR स्कैन करें और कैमरा अनुमति दें, या QR लिंक यहाँ पेस्ट करें।",
    "Your current starting point is": "आपका वर्तमान शुरुआती स्थान",
    "I can help with directions, QR scanning, step-free routes, or finding a department.": "मैं दिशा-निर्देश, QR स्कैन, सीढ़ी-मुक्त रास्ते या विभाग खोजने में मदद कर सकता हूँ।",
    "Take {connector} from {from} to {to}.": "{from} से {to} तक {connector} लें।",
    "Take {connector} to {to}.": "{to} तक {connector} लें।",
    "Follow the corridor for {distance} m towards {target}.": "गलियारे में {distance} मीटर आगे चलें। अगला स्थान: {target}।",
    "You have arrived at {destination}.": "आप {destination} पहुँच गए हैं।",
    "Directions to {destination}": "{destination} तक का रास्ता",
    "From {start} ({floor}) to {destination} · {destinationFloor}": "शुरुआत: {start} ({floor}) · गंतव्य: {destination} · {destinationFloor}",
    "Lift A": "लिफ्ट A", "Stairs": "सीढ़ियाँ",
    "About {minutes} min": "लगभग {minutes} मिनट",
    "Use my phone location": "मेरे फ़ोन की लोकेशन इस्तेमाल करें", "Stop location tracking": "लोकेशन ट्रैकिंग रोकें",
    "Live phone location": "फ़ोन की लाइव लोकेशन", "Location accuracy": "लोकेशन की सटीकता",
    "GPS helps outdoors. Indoors it can drift; routes still start from your selected QR checkpoint.": "GPS बाहर मदद करता है। इमारत के अंदर सटीकता कम हो सकती है; रास्ता चुने हुए QR स्थान से शुरू होगा।",
    "Location permission requires a secure HTTPS page. Open the secure Aura link or use a QR checkpoint.": "लोकेशन अनुमति के लिए सुरक्षित HTTPS पेज ज़रूरी है। सुरक्षित Aura लिंक खोलें या QR स्थान इस्तेमाल करें।",
    "Location permission was denied. Allow location access in your browser settings and try again.": "लोकेशन अनुमति नहीं मिली। ब्राउज़र सेटिंग में अनुमति दें और फिर कोशिश करें।",
    "Could not get your location. Move near a window or try again.": "आपकी लोकेशन नहीं मिली। खिड़की के पास जाएँ या फिर कोशिश करें।",
    "Waiting for GPS…": "GPS का इंतज़ार है…", "GPS unavailable": "GPS उपलब्ध नहीं",
    "Open current location in Google Maps": "वर्तमान लोकेशन Google Maps में खोलें",
    "Plan a route to this location": "इस स्थान के लिए रास्ता बनाएँ",
    "Basement 2 · Oncology & Nuclear Medicine": "बेसमेंट २ · ऑन्कोलॉजी और न्यूक्लियर मेडिसिन",
    "Basement 1 · Imaging & Laboratories": "बेसमेंट १ · इमेजिंग और प्रयोगशालाएँ",
    "Ground Floor · Emergency & Patient Access": "भूतल · आपातकालीन सेवा और मरीज प्रवेश",
    "First Floor · Specialist OPD & Rehabilitation": "पहली मंज़िल · विशेषज्ञ ओपीडी और पुनर्वास",
    "Second Floor · Mother & Child": "दूसरी मंज़िल · मातृ एवं शिशु सेवा",
    "Third Floor · Heart & Kidney Care": "तीसरी मंज़िल · हृदय और गुर्दा देखभाल",
    "Fourth Floor · Surgical Suites": "चौथी मंज़िल · शल्य चिकित्सा विभाग",
    "Fifth Floor · Neurology & Medical Critical Care": "पाँचवीं मंज़िल · न्यूरोलॉजी और मेडिकल क्रिटिकल केयर",
    "Sixth Floor · General Inpatient Wards": "छठी मंज़िल · सामान्य भर्ती वार्ड",
    "Seventh Floor · Private Inpatient Rooms": "सातवीं मंज़िल · निजी भर्ती कक्ष",
    "Eighth Floor · Executive & International Care": "आठवीं मंज़िल · एग्ज़ीक्यूटिव और अंतरराष्ट्रीय सेवा",
    "Ninth Floor · Mental Health & Administration": "नौवीं मंज़िल · मानसिक स्वास्थ्य और प्रशासन",
    "Illustrative demo map · verify on-site": "डेमो नक्शा · अस्पताल में सत्यापित करें",
    "STEP 3 · FOLLOW YOUR JOURNEY": "चरण ३ · अपने रास्ते पर चलें",
    "Routes use the illustrative demo layout. Confirm directions with hospital staff.": "रास्ते उदाहरण वाले नक्शे पर आधारित हैं। अस्पताल के कर्मचारियों से दिशा की पुष्टि करें।",
    "Indoor routes are based on the building’s published map. QR checkpoints set your current location; your browser does not track your position between scans. For emergencies, contact hospital staff.": "अंदरूनी रास्ते डेमो नक्शे पर आधारित हैं। QR कोड शुरुआती जगह तय करता है; अंदर GPS सटीक न हो सकता है। आपातकाल में अस्पताल कर्मचारियों से संपर्क करें।",
    "From": "से", "to": "तक", "your starting point": "आपका शुरुआती स्थान", "current floor": "वर्तमान मंज़िल",
    "Navigation help · Available here": "नेविगेशन सहायता · उपलब्ध",
    "Close help assistant": "सहायता बंद करें",
    "This built-in assistant uses local navigation help and does not send chats to an external AI service.": "यह सहायक स्थानीय जानकारी इस्तेमाल करता है और चैट किसी बाहरी AI सेवा को नहीं भेजता।",
    "No published floor points are available.": "इस मंज़िल के लिए नक्शे के स्थान उपलब्ध नहीं हैं।",
    "The demo floor plan is illustrative and must be verified on site.": "यह उदाहरण के लिए बनाया गया नक्शा है; अस्पताल में सत्यापन ज़रूरी है।",
    "the next corridor junction": "अगले गलियारे का मोड़",
    "the next waypoint": "अगला मार्ग बिंदु",
    "Your location": "आपका स्थान", Destination: "गंतव्य", Route: "रास्ता",
    "Recommended route": "सुझाया गया रास्ता", "map points": "नक्शे के बिंदु",
    "Hospital floor map": "अस्पताल का मंज़िल नक्शा", "Remove destination": "गंतव्य हटाएँ",
    "Matching destinations": "मिलते-जुलते स्थान", "Clear destination search": "गंतव्य खोज मिटाएँ",
    "No active location checkpoints": "अभी कोई स्थान चेकपॉइंट उपलब्ध नहीं है", Floor: "मंज़िल",
    Reception: "स्वागत कक्ष", Cardiology: "हृदय रोग विभाग",
    Department: "विभाग", Lab: "प्रयोगशाला", Emergency: "आपातकालीन सेवा", Billing: "बिलिंग",
    Pharmacy: "फ़ार्मेसी", Room: "कमरा", "Information Desk": "सूचना कक्ष",
    Administration: "प्रशासन", Entrance: "प्रवेश द्वार", Exit: "निकास",
    Cafe: "कैफ़े", "Conference Room": "सम्मेलन कक्ष", "Medical Store": "दवा की दुकान", Office: "कार्यालय",
    "Map north direction": "नक्शे की उत्तर दिशा",
    "Connecting to hospital navigation…": "अस्पताल नेविगेशन से जुड़ रहे हैं…",
    "Navigation is temporarily unavailable": "नेविगेशन अभी उपलब्ध नहीं है",
    "Try again": "फिर से कोशिश करें", "Check that the API and database are running, then retry.": "API और डेटाबेस चल रहे हैं या नहीं जाँचें, फिर दोबारा कोशिश करें।",
    "Choose a starting location and a destination to calculate your route.": "रास्ता निकालने के लिए शुरुआती स्थान और गंतव्य चुनें।",
    "Show {floor} map": "{floor} का नक्शा दिखाएँ",
    "✓ Real shortest-path directions": "✓ सही रास्ता मार्गदर्शन",
    "✓ QR location checkpoints": "✓ QR स्थान बिंदु",
    "✓ Step-free route option": "✓ सीढ़ी-मुक्त रास्ता",
  },
};

const languageNames = { en: "English", mr: "मराठी", hi: "हिन्दी" };
const DemoHospitalFloorPlanNames = [
  "Basement 2 · Oncology & Nuclear Medicine",
  "Basement 1 · Imaging & Laboratories",
  "Ground Floor · Emergency & Patient Access",
  "First Floor · Specialist OPD & Rehabilitation",
  "Second Floor · Mother & Child",
  "Third Floor · Heart & Kidney Care",
  "Fourth Floor · Surgical Suites",
  "Fifth Floor · Neurology & Medical Critical Care",
  "Sixth Floor · General Inpatient Wards",
  "Seventh Floor · Private Inpatient Rooms",
  "Eighth Floor · Executive & International Care",
  "Ninth Floor · Mental Health & Administration",
];

function readQrToken(value) {
  const raw = value.trim();
  if (/^[a-zA-Z0-9]{22}$/.test(raw)) return raw;

  try {
    const url = new URL(raw, window.location.origin);
    const queryToken = url.searchParams.get("qr");
    if (queryToken) return queryToken;

    const parts = url.pathname.split("/").filter(Boolean);
    const qrIndex = parts.findIndex((part) => part === "q");
    if (qrIndex >= 0) return parts[qrIndex + 1] || null;

    const locationIndex = parts.findIndex((part) => part === "location");
    if (locationIndex >= 0) return parts[locationIndex + 1] || null;
  } catch {
    return null;
  }

  return null;
}

function formatDistance(meters) {
  if (meters < 1) return `${Math.round(meters * 100)} cm`;
  return `${Math.round(meters)} m`;
}

function formatDuration(seconds) {
  const minutes = Math.max(1, Math.ceil(seconds / 60));
  return `${minutes} min`;
}

function App() {
  const [initial] = useState(() => {
    const params = new URLSearchParams(window.location.search);
    const path = window.location.pathname.split("/").filter(Boolean);
    const navigateIndex = path.findIndex((part) => part === "navigate");
    const qrIndex = path.findIndex((part) => part === "q");
    const locationIndex = path.findIndex((part) => part === "location");
    return {
      buildingSlug: params.get("building") ||
        (navigateIndex >= 0 ? path[navigateIndex + 1] : null) ||
        "abc-hospital",
      qrToken: params.get("qr") ||
        (qrIndex >= 0 ? path[qrIndex + 1] : null) ||
        (locationIndex >= 0 ? path[locationIndex + 1] : null),
    };
  });
  const [tenantSlug, setTenantSlug] = useState(initial.buildingSlug);
  const [building, setBuilding] = useState(null);
  const [buildingError, setBuildingError] = useState("");
  const [loadingBuilding, setLoadingBuilding] = useState(true);
  const [search, setSearch] = useState("");
  const [destinationId, setDestinationId] = useState("");
  const [startNodeId, setStartNodeId] = useState("");
  const [accessibleOnly, setAccessibleOnly] = useState(false);
  const [activeFloorId, setActiveFloorId] = useState("");
  const [route, setRoute] = useState(null);
  const [routeError, setRouteError] = useState("");
  const [calculatingRoute, setCalculatingRoute] = useState(false);
  const [scannerOpen, setScannerOpen] = useState(false);
  const [scanError, setScanError] = useState("");
  const [manualQr, setManualQr] = useState("");
  const [resolvingQr, setResolvingQr] = useState(false);
  const [menuOpen, setMenuOpen] = useState(false);
  const [language, setLanguage] = useState(() => {
    const saved = window.localStorage.getItem("aura-navigation-language");
    return saved && saved in translations ? saved : "en";
  });
  const [assistantOpen, setAssistantOpen] = useState(false);
  const [assistantInput, setAssistantInput] = useState("");
  const [assistantMessages, setAssistantMessages] = useState([]);
  const [gpsPosition, setGpsPosition] = useState(null);
  const [gpsError, setGpsError] = useState("");
  const [gpsTracking, setGpsTracking] = useState(false);
  const videoRef = useRef(null);
  const gpsWatchId = useRef(null);
  const t = (text) => translations[language][text] ?? text;

  function changeLanguage(nextLanguage) {
    setLanguage(nextLanguage);
    window.localStorage.setItem("aura-navigation-language", nextLanguage);
  }

  function localizeStartLabel(point) {
    if (!point) return "";
    return /^Lift A\b/.test(point.label) ? t("Lift A") : t(point.label);
  }

  function toggleGpsTracking() {
    if (gpsWatchId.current !== null) {
      navigator.geolocation.clearWatch(gpsWatchId.current);
      gpsWatchId.current = null;
      setGpsTracking(false);
      return;
    }
    if (!window.isSecureContext || !navigator.geolocation) {
      setGpsError(t("Location permission requires a secure HTTPS page. Open the secure Aura link or use a QR checkpoint."));
      return;
    }
    setGpsError("");
    setGpsTracking(true);
    gpsWatchId.current = navigator.geolocation.watchPosition(
      (position) => {
        setGpsPosition({
          latitude: position.coords.latitude,
          longitude: position.coords.longitude,
          accuracy: Math.round(position.coords.accuracy),
        });
        setGpsError("");
      },
      (error) => {
        const message = error.code === error.PERMISSION_DENIED
          ? "Location permission was denied. Allow location access in your browser settings and try again."
          : "Could not get your location. Move near a window or try again.";
        setGpsError(t(message));
        setGpsTracking(false);
        if (gpsWatchId.current !== null) navigator.geolocation.clearWatch(gpsWatchId.current);
        gpsWatchId.current = null;
      },
      { enableHighAccuracy: true, maximumAge: 5000, timeout: 15000 }
    );
  }

  useEffect(() => () => {
    if (gpsWatchId.current !== null && navigator.geolocation) {
      navigator.geolocation.clearWatch(gpsWatchId.current);
    }
  }, []);

  function askAssistant(question) {
    const query = question.trim();
    if (!query) return;
    const normalized = query.toLowerCase();
    const match = building?.locations.find((location) => {
      const searchable = `${location.name} ${location.category} ${location.keywords ?? ""}`.toLowerCase();
      return searchable.includes(normalized) ||
        location.name.toLowerCase().split(/[\s/&()-]+/).some(word => word.length > 3 && normalized.includes(word));
    });
    let answer;
    let matchedLocation;
    if (/qr|scan|स्कॅन|स्कैन/.test(normalized)) {
      answer = t("Tap Scan a location QR code and allow camera access, or paste the QR link manually.");
    } else if (/step|lift|wheel|accessible|पायऱ्या|लिफ्ट|सीढ़ी|लिफ्ट/.test(normalized)) {
      answer = t("Avoid stairs and prefer lifts");
    } else if (match) {
      matchedLocation = match;
      answer = `${match.name} · ${t(match.floorName)}. ${t("Choose a starting point, search for a department, select it, and tap Show my route.")}`;
    } else if (/route|direction|मार्ग|रास्ता|दिशा/.test(normalized)) {
      answer = t("Choose a starting point, search for a department, select it, and tap Show my route.");
    } else if (/where|start|location|कुठे|ठिकाण|कहाँ|स्थान/.test(normalized)) {
      answer = `${t("Your current starting point is")} ${localizeStartLabel(currentStart) || t("Reception")} · ${currentStart?.floorName ? t(currentStart.floorName) : ""}.`;
    } else {
      answer = t("I can help with directions, QR scanning, step-free routes, or finding a department.");
    }
    setAssistantMessages((messages) => [...messages, { question: query, answer, location: matchedLocation }]);
    setAssistantInput("");
  }

  function translateInstruction(step) {
    const translateFloorNames = (instruction) =>
      DemoHospitalFloorPlanNames.reduce((text, floorName) => text.replaceAll(floorName, t(floorName)), instruction);
    const localizeRouteText = (instruction) =>
      translateFloorNames(instruction)
        .replaceAll("the next corridor junction", t("the next corridor junction"))
        .replaceAll("the next waypoint", t("the next waypoint"))
        .replaceAll("Lift A", t("Lift A"))
        .replace(/\b(?:the )?(?:stairs?|staircase)(?:\s+(\d+))?\b/gi, (_, number) =>
          number ? `${t("Stairs")} ${number}` : t("Stairs"));
    const lift = step.instruction.match(/^Take (.+) from (.+) to (.+)\.$/);
    if (lift) return localizeRouteText(t("Take {connector} from {from} to {to}.")
      .replace("{connector}", lift[1]).replace("{from}", lift[2]).replace("{to}", lift[3]));
    const stairs = step.instruction.match(/^Take (.+) to (.+)\.$/);
    if (stairs) return localizeRouteText(t("Take {connector} to {to}.")
      .replace("{connector}", stairs[1]).replace("{to}", stairs[2]));
    const corridor = step.instruction.match(/^Follow the corridor for (.+) m towards (.+)\.$/);
    if (corridor) return localizeRouteText(t("Follow the corridor for {distance} m towards {target}.")
      .replace("{distance}", corridor[1]).replace("{target}", corridor[2]));
    const arrival = step.instruction.match(/^You have arrived at (.+)\.$/);
    if (arrival) return localizeRouteText(t("You have arrived at {destination}.").replace("{destination}", arrival[1]));
    return localizeRouteText(step.instruction);
  }

  const refreshBuilding = useCallback(async (slug) => {
    setLoadingBuilding(true);
    setBuildingError("");
    try {
      const response = await loadBuilding(slug);
      setBuilding(response);
    } catch (error) {
      setBuilding(null);
      setBuildingError(error.message);
    } finally {
      setLoadingBuilding(false);
    }
  }, []);

  useEffect(() => {
    let cancelled = false;
    async function initializeVisitor() {
      try {
        let slug = initial.buildingSlug;
        let response = await loadBuilding(slug);
        let resolvedLocation = null;
        if (initial.qrToken) {
          resolvedLocation = await resolveQrToken(initial.qrToken);
          if (resolvedLocation.tenantSlug !== slug) {
            slug = resolvedLocation.tenantSlug;
            response = await loadBuilding(slug);
          }
        }
        if (cancelled) return;
        setTenantSlug(slug);
        setBuilding(response);
        setStartNodeId(
          resolvedLocation?.nodeId ??
          response.startPoints.find((point) => point.label.toLowerCase().includes("reception"))?.nodeId ??
          response.startPoints[0]?.nodeId ??
          ""
        );
        setActiveFloorId(
          resolvedLocation?.floor.id ??
          response.startPoints[0]?.floorId ??
          response.floors[0]?.id ??
          ""
        );
      } catch (error) {
        if (!cancelled) setBuildingError(error.message);
      } finally {
        if (!cancelled) setLoadingBuilding(false);
      }
    }
    void initializeVisitor();
    return () => {
      cancelled = true;
    };
  }, [initial]);

  const applyQrCode = useCallback(
    async (value) => {
      const token = readQrToken(value);
      if (!token) {
        setScanError("That QR code does not contain a supported navigation link. Try a hospital location QR code.");
        return;
      }

      setResolvingQr(true);
      setScanError("");
      try {
        const location = await resolveQrToken(token);
        if (location.tenantSlug !== tenantSlug) {
          const nextBuilding = await loadBuilding(location.tenantSlug);
          setBuilding(nextBuilding);
          setTenantSlug(location.tenantSlug);
        }
        setStartNodeId(location.nodeId);
        setActiveFloorId(location.floor.id);
        setRoute(null);
        setRouteError("");
        setManualQr("");
        setScannerOpen(false);
        window.history.replaceState(
          {},
          "",
          `/?building=${encodeURIComponent(location.tenantSlug)}&qr=${encodeURIComponent(token)}`
        );
      } catch (error) {
        setScanError(error.message);
      } finally {
        setResolvingQr(false);
      }
    },
    [tenantSlug]
  );

  useEffect(() => {
    if (!scannerOpen || !videoRef.current) return undefined;
    let stopped = false;
    let controls;
    import("@zxing/browser")
      .then(({ BrowserQRCodeReader }) => {
        if (stopped || !videoRef.current) return undefined;
        const reader = new BrowserQRCodeReader();
        return reader.decodeFromVideoDevice(undefined, videoRef.current, (result, error) => {
          if (result && !stopped) {
            stopped = true;
            controls?.stop();
            void applyQrCode(result.getText());
            return;
          }

          if (error && error.name !== "NotFoundException" && error.name !== "ChecksumException" &&
              error.name !== "FormatException" && !stopped) {
            setScanError("The camera could not read a QR code. Check camera permission or enter the QR link below.");
          }
        });
      })
      .then((scannerControls) => {
        if (!scannerControls) return;
        controls = scannerControls;
        if (stopped) controls.stop();
      })
      .catch(() => {
        if (!stopped) {
          setScanError("Camera access is unavailable. Allow camera permission, use HTTPS, or enter the QR link below.");
        }
      });

    return () => {
      stopped = true;
      controls?.stop();
    };
  }, [applyQrCode, scannerOpen]);

  const filteredLocations = useMemo(() => {
    if (!building) return [];
    const query = search.trim().toLowerCase();
    if (!query) return building.locations;
    return building.locations.filter((location) =>
      [location.name, location.category, location.floorName, location.code, location.keywords]
        .filter(Boolean)
        .some((value) => value.toLowerCase().includes(query))
    );
  }, [building, search]);

  const currentStart =
    building?.startPoints.find((point) => point.nodeId === startNodeId) ??
    building?.startPoints.find((point) => point.label.toLowerCase().includes("reception")) ??
    building?.startPoints[0];
  const effectiveStartNodeId = currentStart?.nodeId ?? "";
  const activeFloor =
    building?.floors.find((floor) => floor.id === activeFloorId) ??
    building?.floors.find((floor) => floor.id === currentStart?.floorId) ??
    building?.floors[0];
  const displayedFloorId = activeFloor?.id ?? "";
  const selectedDestination = building?.locations.find((location) => location.id === destinationId);
  const nodeById = useMemo(
    () => new Map((building?.nodes ?? []).map((node) => [node.id, node])),
    [building]
  );
  const locationById = useMemo(
    () => new Map((building?.locations ?? []).map((location) => [location.id, location])),
    [building]
  );

  async function handleFindRoute(event) {
    event.preventDefault();
    if (!effectiveStartNodeId || !destinationId) {
      setRouteError(t("Choose a starting location and a destination to calculate your route."));
      return;
    }

    setCalculatingRoute(true);
    setRouteError("");
    setRoute(null);
    try {
      const result = await findRoute(tenantSlug, {
        startNodeId: effectiveStartNodeId,
        destinationLocationId: destinationId,
        accessibleOnly,
      });
      setRoute(result);
      setActiveFloorId(currentStart?.floorId ?? result.legs[0]?.fromFloorId ?? displayedFloorId);
    } catch (error) {
      setRouteError(error.message);
    } finally {
      setCalculatingRoute(false);
    }
  }

  function chooseDestination(location) {
    setDestinationId(location.id);
    setSearch(location.name);
    setRoute(null);
    setRouteError("");
  }

  function changeStartingPoint(nodeId) {
    setStartNodeId(nodeId);
    setRoute(null);
    setRouteError("");
    const point = building?.startPoints.find((startPoint) => startPoint.nodeId === nodeId);
    if (point) setActiveFloorId(point.floorId);
  }

  function handleFloorChange(floorId) {
    setActiveFloorId(floorId);
  }

  const visibleEdges = building?.edges.filter((edge) => {
    const from = nodeById.get(edge.fromNodeId);
    const to = nodeById.get(edge.toNodeId);
    return from?.floorId === displayedFloorId && to?.floorId === displayedFloorId;
  }) ?? [];
  const routeLegs = route?.legs.filter(
    (leg) => leg.fromFloorId === displayedFloorId && leg.toFloorId === displayedFloorId
  ) ?? [];
  const floorNodes = building?.nodes.filter((node) => node.floorId === displayedFloorId) ?? [];

  if (loadingBuilding && !building) {
    return (
      <div className="navigation-app loading-screen">
        <div className="loading-card">
          <span className="brand-mark" aria-hidden="true">+</span>
          <p>{t("Connecting to hospital navigation…")}</p>
        </div>
      </div>
    );
  }

  if (buildingError && !building) {
    return (
      <div className="navigation-app loading-screen">
        <div className="loading-card error-card" role="alert">
          <span className="brand-mark" aria-hidden="true">!</span>
          <h1>{t("Navigation is temporarily unavailable")}</h1>
          <p>{buildingError}</p>
          <button className="primary-button" onClick={() => void refreshBuilding(tenantSlug)}>
            {t("Try again")}
          </button>
          <small>{t("Check that the API and database are running, then retry.")}</small>
        </div>
      </div>
    );
  }

  return (
    <div className="navigation-app" lang={language}>
      <header className="topbar">
        <a className="brand" href="#home" aria-label={`${building.name} visitor navigation home`}>
          <span className="brand-mark" aria-hidden="true">+</span>
          <span>
            <strong><span className="brand-aura">{building.name.split(" ")[0]}</span> <span className="brand-hospital">{building.name.split(" ").slice(1).join(" ")}</span></strong>
            <small>{t("INDOOR WAYFINDING")}</small>
          </span>
        </a>
        <nav className={menuOpen ? "nav-links nav-open" : "nav-links"} aria-label={t("Main navigation")}>
          <a href="#navigate" onClick={() => setMenuOpen(false)}>{t("Find a location")}</a>
          <a href="#map" onClick={() => setMenuOpen(false)}>{t("Floor map")}</a>
          <a href="#directions" onClick={() => setMenuOpen(false)}>{t("Directions")}</a>
        </nav>
        <div className="header-status"><span className="status-dot" /> {t("Visitor navigation")}</div>
        <div className="menu-wrap">
          <button
            className="mobile-menu"
            onClick={() => setMenuOpen((open) => !open)}
            aria-expanded={menuOpen}
            aria-label={t("Main navigation")}
            aria-controls="visitor-menu"
          >
            ☰
          </button>
          {menuOpen && (
            <div className="visitor-menu" id="visitor-menu">
              <label htmlFor="language-select">{t("Choose language")}</label>
              <select id="language-select" value={language} onChange={(event) => changeLanguage(event.target.value)}>
                {Object.entries(languageNames).map(([code, name]) => <option key={code} value={code}>{name}</option>)}
              </select>
              <button type="button" onClick={() => { setAssistantOpen(true); setMenuOpen(false); }}>✦ {t("Help assistant")}</button>
            </div>
          )}
        </div>
      </header>

      <main className="navigation-main" id="home">
        <section className="welcome">
          <div className="welcome-copy">
            <div className="eyebrow"><span className="eyebrow-dot" /> AURA HOSPITAL · PUNE</div>
            <h1>{t("Find your way.")}<br /><span>{t("Feel taken care of.")}</span></h1>
            <p>{t(building.welcomeMessage || "Find hospital departments and follow clear, floor-by-floor directions.")}</p>
            <a className="start-directions-button" href="#navigate">{t("Start with directions")} <span aria-hidden="true">→</span></a>
            <div className="welcome-tags">
              <span>{t("✓ Real shortest-path directions")}</span>
              <span>{t("✓ QR location checkpoints")}</span>
              <span>{t("✓ Step-free route option")}</span>
            </div>
          </div>
          <div className="hero-decoration" aria-hidden="true">
            <div className="hero-orbit" />
            <div className="hero-building"><span>+</span><i /><i /><i /><i /><i /><i /></div>
            <span className="hero-pin">A</span>
          </div>
        </section>

        <section className="planner-grid" id="navigate">
          <aside className="planner-card">
            <div className="section-heading">
              <span className="heading-icon" aria-hidden="true">⌖</span>
              <div>
                <span className="section-eyebrow">{t("PLAN YOUR VISIT")}</span>
                <h2>{t("Where do you need to go?")}</h2>
              </div>
            </div>

            <div className="flow-block">
              <div className="flow-title"><span className="flow-number">1</span><strong>{t("Where are you now?")}</strong></div>
              <label className="field-label" htmlFor="start-point">{t("Your current location")}</label>
              <select
                id="start-point"
                value={effectiveStartNodeId}
                onChange={(event) => changeStartingPoint(event.target.value)}
                disabled={!building.startPoints.length}
              >
                {building.startPoints.map((point) => (
                  <option key={point.nodeId} value={point.nodeId}>{localizeStartLabel(point)} · {t(point.floorName)}</option>
                ))}
                {!building.startPoints.length && <option value="">{t("No active location checkpoints")}</option>}
              </select>
              <button
                type="button"
                className="scan-button"
                onClick={() => {
                  setScanError("");
                  setScannerOpen(true);
                }}
              >
                <span aria-hidden="true">▦</span>
                {t("Scan a location QR code")}
                <span className="scan-arrow">↗</span>
              </button>
              {currentStart && (
                <div className="current-location-note">
                  <span className="location-dot" />
                  {t("Starting at")} <strong>{localizeStartLabel(currentStart)}</strong> · {t(currentStart.floorName)}
                </div>
              )}
              <button type="button" className="gps-button" onClick={toggleGpsTracking}>
                <span aria-hidden="true">{gpsTracking ? "◉" : "◎"}</span>
                {t(gpsTracking ? "Stop location tracking" : "Use my phone location")}
              </button>
              {gpsPosition && (
                <div className="gps-position" role="status">
                  <span className="gps-live-dot" />
                  <span>
                    <strong>{t("Live phone location")}</strong>
                    <small>{gpsPosition.latitude.toFixed(5)}, {gpsPosition.longitude.toFixed(5)} · ±{gpsPosition.accuracy} m</small>
                    <a href={`https://www.google.com/maps/search/?api=1&query=${gpsPosition.latitude},${gpsPosition.longitude}`} target="_blank" rel="noreferrer">{t("Open current location in Google Maps")} ↗</a>
                  </span>
                </div>
              )}
              {gpsError && <p className="inline-error" role="alert">{gpsError}</p>}
              <p className="gps-disclaimer">{t("GPS helps outdoors. Indoors it can drift; routes still start from your selected QR checkpoint.")}</p>
            </div>

            <form className="flow-block destination-form" onSubmit={handleFindRoute}>
              <div className="flow-title"><span className="flow-number">2</span><strong>{t("Choose your destination")}</strong></div>
              <label className="field-label" htmlFor="destination-search">{t("Search departments and services")}</label>
              <div className="search-box">
                <span aria-hidden="true">⌕</span>
                <input
                  id="destination-search"
                  type="search"
                  autoComplete="off"
                  value={search}
                  onChange={(event) => {
                    setSearch(event.target.value);
                    setDestinationId("");
                    setRoute(null);
                  }}
                  placeholder={t("Try Cardiology or Pharmacy")}
                />
                {search && (
                  <button
                    type="button"
                    className="clear-search"
                    aria-label={t("Clear destination search")}
                    onClick={() => {
                      setSearch("");
                      setDestinationId("");
                      setRoute(null);
                    }}
                  >
                    ×
                  </button>
                )}
              </div>
              {search && !destinationId && (
                <div className="search-results" role="listbox" aria-label={t("Matching destinations")}>
                  {filteredLocations.length ? filteredLocations.slice(0, 6).map((location) => (
                    <button
                      type="button"
                      className="result-item"
                      key={location.id}
                      onClick={() => chooseDestination(location)}
                    >
                      <span className="result-icon" aria-hidden="true">{location.icon || "⌖"}</span>
                      <span className="result-copy">
                        <strong>{location.name}</strong>
                        <small>{t(location.category)} · {t(location.floorName)}</small>
                      </span>
                      <span className="result-arrow" aria-hidden="true">↗</span>
                    </button>
                  )) :                   <p className="empty-result">{t("No locations match")} “{search}”.</p>}
                </div>
              )}
              {selectedDestination && (
                <div className="selected-destination">
                  <span className="selected-check" aria-hidden="true">✓</span>
                  <span><strong>{selectedDestination.name}</strong><small>{t(selectedDestination.floorName)} · {t(selectedDestination.category)}</small></span>
                  <button type="button" aria-label={t("Remove destination")} onClick={() => { setDestinationId(""); setSearch(""); setRoute(null); }}>×</button>
                </div>
              )}

              <label className="accessibility-box">
                <span className="accessibility-symbol" aria-hidden="true">♿</span>
                <span className="accessibility-copy">
                  <strong>{t("Step-free route")}</strong>
                  <small>{t("Avoid stairs and prefer lifts")}</small>
                </span>
                <input
                  type="checkbox"
                  checked={accessibleOnly}
                  onChange={(event) => {
                    setAccessibleOnly(event.target.checked);
                    setRoute(null);
                  }}
                />
                <span className="toggle-track" aria-hidden="true"><span /></span>
              </label>

              {routeError && <p className="inline-error" role="alert">{routeError}</p>}
              <button
                className="primary-button"
                type="submit"
                disabled={!effectiveStartNodeId || !destinationId || calculatingRoute}
              >
                {calculatingRoute ? t("Finding the best route…") : t("Show my route")}
                {!calculatingRoute && <span aria-hidden="true">→</span>}
              </button>
            </form>
          </aside>

          <section className="map-panel" id="map">
            <div className="map-header">
              <div>
                <span className="section-eyebrow">{t("LIVE ROUTE PREVIEW")}</span>
                <h2>{activeFloor ? t(activeFloor.name) : t("Hospital floor map")}</h2>
                <p>{route ? t("Directions to {destination}").replace("{destination}", route.destination) : t("Choose a floor to explore hospital locations")}</p>
              </div>
              <span className="map-status"><span /> {t("Illustrative demo map · verify on-site")}</span>
            </div>

            <div className="floor-tabs" role="tablist" aria-label={t("Select a floor")}>
              {building.floors.map((floor) => (
                <button
                  key={floor.id}
                  type="button"
                  role="tab"
                  aria-selected={floor.id === displayedFloorId}
                  className={floor.id === displayedFloorId ? "floor-tab active" : "floor-tab"}
                  onClick={() => handleFloorChange(floor.id)}
                >
                  <span>{floor.code}</span>{t(floor.name)}
                  {route?.legs.some((leg) => leg.fromFloorId === floor.id || leg.toFloorId === floor.id) && (
                    <i className="route-floor-dot" aria-label="Route passes this floor" />
                  )}
                </button>
              ))}
            </div>

            <div className="map-canvas">
              <div className="map-grid" aria-hidden="true" />
              {displayedFloorId && (
                <svg
                  className="floor-map-svg"
                  viewBox="0 0 1000 700"
                  role="img"
                  aria-label={`${activeFloor ? t(activeFloor.name) : t("Floor")} map showing hospital corridors and destinations`}
                >
                  <g className="map-corridors">
                    {visibleEdges.map((edge, index) => {
                      const from = nodeById.get(edge.fromNodeId);
                      const to = nodeById.get(edge.toNodeId);
                      if (!from || !to) return null;
                      return (
                        <line
                          key={`${edge.fromNodeId}-${edge.toNodeId}-${index}`}
                          x1={from.x} y1={from.y} x2={to.x} y2={to.y}
                          className={edge.type.toLowerCase() === "lift" || edge.type.toLowerCase() === "stair" ? "connector-line" : ""}
                        />
                      );
                    })}
                  </g>
                  <g className="map-route">
                    {routeLegs.map((leg) => (
                      <line
                        key={`${leg.fromNodeId}-${leg.toNodeId}`}
                        x1={leg.fromX} y1={leg.fromY} x2={leg.toX} y2={leg.toY}
                        className="route-line"
                      />
                    ))}
                  </g>
                  <g className="map-node-layer">
                    {floorNodes.map((node) => {
                      const place = node.locationId ? locationById.get(node.locationId) : null;
                      const isCurrent = node.id === effectiveStartNodeId;
                      const isDestination = place?.id === destinationId;
                      const mapLabel = isCurrent
                        ? `You · ${place?.name ?? node.connectorName ?? localizeStartLabel(currentStart) ?? "here"}`
                        : place?.name ?? node.connectorName;
                      return (
                        <g
                          key={node.id}
                          className={`map-node${isCurrent ? " map-node-current" : ""}${isDestination ? " map-node-destination" : ""}`}
                          transform={`translate(${node.x} ${node.y})`}
                        >
                          <circle r={isCurrent || isDestination ? 15 : place || node.connectorName ? 10 : 5} />
                          {mapLabel && (
                            <text x="0" y="-19" textAnchor="middle" textLength="150" lengthAdjust="spacingAndGlyphs">
                              {mapLabel.length > 22 ? `${mapLabel.slice(0, 21)}…` : mapLabel}
                            </text>
                          )}
                        </g>
                      );
                    })}
                  </g>
                </svg>
              )}
              {!floorNodes.length && <p className="map-empty">{t("No published floor points are available.")}</p>}
              <div className="map-compass" aria-label={t("Map north direction")}>N<br />↑</div>
              {route && <div className="map-legend"><span /> {t("Recommended route")}</div>}
            </div>

            <div className="map-footer">
              <span><i className="legend-current" /> {t("Your location")}</span>
              <span><i className="legend-destination" /> {t("Destination")}</span>
              <span><i className="legend-route" /> {t("Route")}</span>
              <strong>{floorNodes.length} {t("map points")}</strong>
            </div>
          </section>
        </section>

        <section className="journey-panel" id="directions" aria-live="polite">
          <div className="journey-heading">
            <div>
              <span className="section-eyebrow">{t("STEP 3 · FOLLOW YOUR JOURNEY")}</span>
              <h2>{route ? t("Directions to {destination}").replace("{destination}", route.destination) : t("Your directions will appear here")}</h2>
              <p>{route ? t("From {start} ({floor}) to {destination} · {destinationFloor}")
                .replace("{start}", localizeStartLabel(currentStart) || t("your starting point"))
                .replace("{floor}", currentStart?.floorName ? t(currentStart.floorName) : t("current floor"))
                .replace("{destination}", route.destination)
                .replace("{destinationFloor}", t(route.floor)) : t("Select your starting location and a destination, then show your route.")}</p>
            </div>
            {route && <span className="route-ready"><i /> {t("Route calculated")}</span>}
          </div>

          {route ? (
            <>
              <div className="route-summary">
                <div className="summary-item"><span className="summary-icon">↗</span><small>{t("DISTANCE")}</small><strong>{formatDistance(route.distanceMeters)}</strong></div>
                <div className="summary-item"><span className="summary-icon">◷</span><small>{t("WALKING TIME")}</small><strong>{t("About {minutes} min").replace("{minutes}", formatDuration(route.travelSeconds).replace(" min", ""))}</strong></div>
                <div className="summary-item"><span className="summary-icon">♿</span><small>{t("ROUTE")}</small><strong>{route.accessible ? t("Step-free route") : t("Fastest available")}</strong></div>
              </div>
              <ol className="directions-list">
                {route.steps.map((step) => (
                  <li key={`${step.number}-${step.floorId}`}>
                    <button type="button" onClick={() => setActiveFloorId(step.floorId)} aria-label={t("Show {floor} map").replace("{floor}", t(step.floorName))}>
                      <span className="step-number">{step.number}</span>
                      <span className="step-copy">
                        <strong>{translateInstruction(step)}</strong>
                        <small>{t(step.floorName)}{step.distanceMeters > 0 ? ` · ${formatDistance(step.distanceMeters)}` : ""}</small>
                      </span>
                      <span className="step-open" aria-hidden="true">↗</span>
                    </button>
                  </li>
                ))}
              </ol>
            </>
          ) : (
            <div className="journey-placeholder">
              <span aria-hidden="true">⌖</span>
              <p>{t("Routes use the illustrative demo layout. Confirm directions with hospital staff.")}</p>
            </div>
          )}
          <div className="route-disclaimer" id="help">
            <span aria-hidden="true">ⓘ</span>
            <p><strong>{t("Visitor guidance:")}</strong> {t("Indoor routes are based on the building’s published map. QR checkpoints set your current location; your browser does not track your position between scans. For emergencies, contact hospital staff.")}</p>
          </div>
        </section>

        <footer className="footer">
          <a className="brand footer-brand" href="#home">
            <span className="brand-mark" aria-hidden="true">+</span>
            <span><strong><span className="brand-aura">{building.name.split(" ")[0]}</span> <span className="brand-hospital">{building.name.split(" ").slice(1).join(" ")}</span></strong><small>INDOOR WAYFINDING</small></span>
          </a>
          <span>{t("Helping every visitor find their way.")}</span>
          <span>Visitor navigation · {building.floors.length} floors</span>
        </footer>
      </main>

      <div className="assistant-widget">
        {assistantOpen && (
          <section className="assistant-panel" role="dialog" aria-modal="false" aria-labelledby="assistant-title">
            <header className="assistant-header">
              <span className="assistant-avatar" aria-hidden="true">✦</span>
              <div><strong id="assistant-title">{t("Aura Help Assistant")}</strong><small>{t("Navigation help · Available here")}</small></div>
              <button type="button" onClick={() => setAssistantOpen(false)} aria-label={t("Close help assistant")}>×</button>
            </header>
            <div className="assistant-messages" aria-live="polite">
              <p className="assistant-bubble">{t("I can help you find departments, use QR checkpoints, and understand your route.")}</p>
              {assistantMessages.map((message, index) => (
                <div className="assistant-exchange" key={`${index}-${message.question}`}>
                  <p className="assistant-user-bubble">{message.question}</p>
                  <p className="assistant-bubble">{message.answer}</p>
                  {message.location && (
                    <button
                      className="assistant-route-button"
                      type="button"
                      onClick={() => {
                        chooseDestination(message.location);
                        setAssistantOpen(false);
                      }}
                    >
                      {t("Plan a route to this location")} <span aria-hidden="true">→</span>
                    </button>
                  )}
                </div>
              ))}
            </div>
            <div className="assistant-prompts">
              {["How do I get directions?", "How do I scan a QR code?", "Where am I?"].map((prompt) => (
                <button type="button" key={prompt} onClick={() => askAssistant(prompt)}>{t(prompt)}</button>
              ))}
            </div>
            <form className="assistant-form" onSubmit={(event) => { event.preventDefault(); askAssistant(assistantInput); }}>
              <input value={assistantInput} onChange={(event) => setAssistantInput(event.target.value)} placeholder={t("Ask a question…")} aria-label={t("Ask a question…")} />
              <button type="submit" disabled={!assistantInput.trim()}>{t("Send")} <span aria-hidden="true">→</span></button>
            </form>
            <p className="assistant-note">{t("This built-in assistant uses local navigation help and does not send chats to an external AI service.")}</p>
          </section>
        )}
        <button className="assistant-launcher" type="button" onClick={() => setAssistantOpen((open) => !open)} aria-expanded={assistantOpen}>
          <span aria-hidden="true">✦</span> {assistantOpen ? t("Close help assistant") : t("Ask Aura")}
        </button>
      </div>

      {scannerOpen && (
        <div
          className="scanner-backdrop"
          role="presentation"
          onMouseDown={(event) => {
            if (event.target === event.currentTarget) setScannerOpen(false);
          }}
        >
          <section className="scanner-dialog" role="dialog" aria-modal="true" aria-labelledby="scanner-title">
            <div className="scanner-heading">
              <div><span className="section-eyebrow">UPDATE YOUR STARTING POINT</span><h2 id="scanner-title">Scan a location QR</h2></div>
              <button type="button" className="close-dialog" onClick={() => setScannerOpen(false)} aria-label="Close QR scanner">×</button>
            </div>
            <div className="camera-frame">
              <video ref={videoRef} muted playsInline aria-label="Camera QR code preview" />
              <span className="camera-scan-line" aria-hidden="true" />
              <span className="camera-hint">Point your camera at a hospital wayfinding QR code</span>
            </div>
            {scanError && <p className="inline-error" role="alert">{scanError}</p>}
            {resolvingQr && <p className="scanner-status" role="status">Checking this location…</p>}
            <form
              className="manual-qr-form"
              onSubmit={(event) => {
                event.preventDefault();
                void applyQrCode(manualQr);
              }}
            >
              <label className="field-label" htmlFor="manual-qr">Or paste the QR link / location token</label>
              <div className="manual-qr-row">
                <input
                  id="manual-qr"
                  value={manualQr}
                  onChange={(event) => setManualQr(event.target.value)}
                  placeholder="https://…/?building=abc-hospital&qr=…"
                />
                <button className="primary-button" disabled={!manualQr.trim() || resolvingQr}>Use location</button>
              </div>
            </form>
            <p className="scanner-privacy">Camera video is processed in your browser and is not uploaded.</p>
          </section>
        </div>
      )}
    </div>
  );
}

export default App;
