using eMDR_BatchGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace eMDR_BatchGenerator.App
{
    public class GeradorXmlFDA
    {
        public static TB_FDA_NOTIFICACAO Notificacao { get; set; }

        public GeradorXmlFDA()
        {
            Notificacao = new();
        }

        public XmlDocument GerarExcelDoFDA(List<TB_FDA_NOTIFICACAO> lst, TB_FDA_NOTIFICACAO notificacao)
        {
            Notificacao = notificacao;

            XmlDocument doc = new XmlDocument();

            // Create root element
            XmlElement root = doc.CreateElement("PORR_IN040001UV01", "urn:hl7-org:v3");
            root.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            root.SetAttribute("ITSVersion", "XML_1.0");
            root.SetAttribute("xsi:schemaLocation", "urn:hl7-org:v3 ../../../../eMDRHL7/Impl_Files/Con170227.xsd");
            doc.AppendChild(root);

            // Create child elements
            XmlElement id = doc.CreateElement("id");
            id.SetAttribute("assigningAuthorityName", "MessageSender");
            id.SetAttribute("extension", $"3004201-{DateTime.Now.ToString("yyyyMMddHHmmss")}"); ///"3004201-20240830114908"); //message.IdExtension);
            id.SetAttribute("root", "1.1");
            root.AppendChild(id);

            XmlElement creationTime = doc.CreateElement("creationTime");
            creationTime.SetAttribute("value", "20240830"); //message.CreationTime);
            root.AppendChild(creationTime);

            XmlElement versionCode = doc.CreateElement("versionCode");
            versionCode.SetAttribute("code", "V3NORMED_2016"); //message.VersionCode);
            root.AppendChild(versionCode);
            XmlElement receiverElement, idRepresentedOrganization, nameRepresentedOrganization;

            //Gera tag receiver no início do XML para o FDA
            GerarReceiverDoXMLFDA(doc, out receiverElement, out idRepresentedOrganization, out nameRepresentedOrganization);

            //Gera tag Sender no início do XML para o FDA
            XmlElement sender = GerarSenderXMLFDA(doc, idRepresentedOrganization, nameRepresentedOrganization);

            XmlElement message = GerarMessagesXMLFDA(doc);

            root.AppendChild(receiverElement);
            root.AppendChild(sender);
            root.AppendChild(message);

            return doc;
        }

        private static XmlElement GerarMessagesXMLFDA(XmlDocument doc)
        {
            //------------------------------------------------
            XmlElement messages = doc.CreateElement("message");
            //------------------------------------------------
            XmlElement id = doc.CreateElement("id");
            id.SetAttribute("extension", "1");
            //------------------------------------------------
            XmlElement creationTime = doc.CreateElement("creationTime");
            creationTime.SetAttribute("nullFlavor", "NA");
            //------------------------------------------------
            XmlElement interactionId = doc.CreateElement("interactionId");
            interactionId.SetAttribute("root", "2.16.840.1.113883.1.6");
            interactionId.SetAttribute("extension", "PORR_IN04001");
            interactionId.SetAttribute("assigningAuthorityName", "HL7");
            //------------------------------------------------
            XmlElement processingCode = doc.CreateElement("processingCode");
            processingCode.SetAttribute("nullFlavor", "NA");
            //------------------------------------------------
            XmlElement processingModeCode = doc.CreateElement("processingModeCode");
            processingModeCode.SetAttribute("nullFlavor", "NA");
            //------------------------------------------------
            XmlElement acceptAckCode = doc.CreateElement("acceptAckCode");
            acceptAckCode.SetAttribute("nullFlavor", "NA");
            //------------------------------------------------
            messages.AppendChild(id);
            messages.AppendChild(creationTime);
            messages.AppendChild(interactionId);
            messages.AppendChild(processingCode);
            messages.AppendChild(processingModeCode);
            messages.AppendChild(acceptAckCode);

            XmlElement receiverMessage = GerarReceiverDoMessage(doc);
            XmlElement senderMessage = GerarSenderDoMessage(doc);

            messages.AppendChild(receiverMessage);
            messages.AppendChild(senderMessage);

            XmlElement controlActProcess = GerarControlActProcess(doc);
            messages.AppendChild(controlActProcess);

            return messages;
        }

        private static XmlElement GerarControlActProcess(XmlDocument doc)
        {
            XmlElement controlActProcess = doc.CreateElement("controlActProcess");
            controlActProcess.SetAttribute("moodCode", "EVN");
            //------------------------------------------------
            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "PORR_TE040001UV01");
            code.SetAttribute("codeSystem", "HL7");
            code.SetAttribute("codeSystemName", "HL7 Trigger Event Id");
            //------------------------------------------------
            XmlElement effectiveTime = doc.CreateElement("effectiveTime");
            effectiveTime.SetAttribute("value", "20240830");
            //------------------------------------------------
            controlActProcess.AppendChild(code);
            controlActProcess.AppendChild(effectiveTime);

            XmlElement authorOrPerformer = GerarAuthorOrPerformer(doc);

            controlActProcess.AppendChild(authorOrPerformer);

            //TODO: Implementar possibilidade de criar múltiplos Subjects
            XmlElement subject = GerarSubject(doc);

            controlActProcess.AppendChild(subject);

            GerarReasonsOf(doc, ref controlActProcess);

            return controlActProcess;
        }

        private static void GerarReasonsOf(XmlDocument doc, ref XmlElement controlActProcess)
        {
            //------------------------------------------------
            XmlElement reasonOf = doc.CreateElement("reasonOf");
            XmlElement detectedIssueEvent = doc.CreateElement("detectedIssueEvent");
            XmlElement code = doc.CreateElement("code");
            XmlElement value = doc.CreateElement("value");

            reasonOf.AppendChild(detectedIssueEvent);
            detectedIssueEvent.AppendChild(code);
            detectedIssueEvent.AppendChild(value);

            code.SetAttribute("code", "C53566");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code.SetAttribute("codeSystemName", "Report_Source");
            value.SetAttribute("xsi:type", "CE");
            value.SetAttribute("code", "C53287");
            value.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");

            controlActProcess.AppendChild(reasonOf);

            //------------------------------------------------
            XmlElement reasonOf1 = doc.CreateElement("reasonOf");
            XmlElement detectedIssueEvent1 = doc.CreateElement("detectedIssueEvent");
            XmlElement code1 = doc.CreateElement("code");
            XmlElement value1 = doc.CreateElement("value");

            reasonOf1.AppendChild(detectedIssueEvent1);
            detectedIssueEvent1.AppendChild(code1);
            detectedIssueEvent1.AppendChild(value1);

            code1.SetAttribute("code", "C53571");
            code1.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code1.SetAttribute("codeSystemName", "Type_Of_Report");
            value1.SetAttribute("xsi:type", "CE");
            value1.SetAttribute("code", "C53577");
            value1.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");

            controlActProcess.AppendChild(reasonOf1);

            //------------------------------------------------
            XmlElement reasonOf2 = doc.CreateElement("reasonOf");
            XmlElement detectedIssueEvent2 = doc.CreateElement("detectedIssueEvent");
            XmlElement code2 = doc.CreateElement("code");
            XmlElement value2 = doc.CreateElement("value");

            reasonOf2.AppendChild(detectedIssueEvent2);
            detectedIssueEvent2.AppendChild(code2);
            detectedIssueEvent2.AppendChild(value2);

            code2.SetAttribute("code", "C53570");
            code2.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code2.SetAttribute("codeSystemName", "Type_of_Reportable_Event");
            value2.SetAttribute("xsi:type", "CE");
            value2.SetAttribute("code", "C53569");
            value2.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");

            controlActProcess.AppendChild(reasonOf2);

            //------------------------------------------------
            XmlElement reasonOf3 = doc.CreateElement("reasonOf");
            XmlElement detectedIssueEvent3 = doc.CreateElement("detectedIssueEvent");
            XmlElement code3 = doc.CreateElement("code");
            XmlElement value3 = doc.CreateElement("value");

            reasonOf3.AppendChild(detectedIssueEvent3);
            detectedIssueEvent3.AppendChild(code3);
            detectedIssueEvent3.AppendChild(value3);

            code3.SetAttribute("code", "C53584");
            code3.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code3.SetAttribute("codeSystemName", "Type_of_Follow-up");
            value3.SetAttribute("xsi:type", "CE");
            value3.SetAttribute("code", "C53586");
            value3.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");

            controlActProcess.AppendChild(reasonOf3);

            //------------------------------------------------
            XmlElement reasonOf4 = doc.CreateElement("reasonOf");
            XmlElement detectedIssueEvent4 = doc.CreateElement("detectedIssueEvent");
            XmlElement code4 = doc.CreateElement("code");
            XmlElement value4 = doc.CreateElement("value");

            reasonOf4.AppendChild(detectedIssueEvent4);
            detectedIssueEvent4.AppendChild(code4);
            detectedIssueEvent4.AppendChild(value4);

            code4.SetAttribute("code", "C53584");
            code4.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code4.SetAttribute("codeSystemName", "Type_of_Follow-up");
            value4.SetAttribute("xsi:type", "CE");
            value4.SetAttribute("code", "C53587");
            value4.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");

            controlActProcess.AppendChild(reasonOf4);
        }

        private static XmlElement GerarSubject(XmlDocument doc)
        {
            XmlElement subject = doc.CreateElement("subject");
            XmlElement investigationEvent = doc.CreateElement("investigationEvent");
            subject.AppendChild(investigationEvent);
            //------------------------------------------------
            XmlElement id = doc.CreateElement("id");
            id.SetAttribute("assigningAuthorityName", "FDA");
            id.SetAttribute("extension", $"3004201-2024-{Notificacao.LAUDO}"); //$"3004201-2024-00007");
            id.SetAttribute("root", "2.16.840.1.113883.3.24");

            investigationEvent.AppendChild(id);
            //------------------------------------------------
            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "C41331");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code.SetAttribute("codeSystemName", "Adverse_Event_Or_Product_Problem_Report");

            investigationEvent.AppendChild(code);
            //------------------------------------------------
            XmlElement text = doc.CreateElement("text");
            text.SetAttribute("mediaType", "text/plain");
            text.InnerText = "Osseointegration problems in endosseous dental implants during the healing phase is a known inherent risk of dental implant treatment. Success rates for implants are high, with reports of approximately 95% (Singh et al., 2020) and 97.3% (Schwartz-Arad et al., 2008). The implant failure behavior can take place in the surgical phase or prosthetic phase and/or the immediate load phase. The most common sign for implant failure is implant mobility during the surgical phase, whereas infection and marginal bone loss are the common signs during the prosthetic phase (Schwartz-Arad et al., 2008). In 2014 Chrcanovic performed a review for implant failure analyzing articles from 2004 to 2014 the author declared that: \"We do not know why some implants fail primarily but fortunately the frequency of such failures is small, in the range of 1-2% in most clinical reports. Thus, it can be seen that the percentage of non-osteointegrated implants that S.I.N. was aware of is below the probability predicted in clinical literature of 1-2% of products.";

            investigationEvent.AppendChild(text);
            //------------------------------------------------
            XmlElement statusCode = doc.CreateElement("statusCode");
            investigationEvent.AppendChild(statusCode);
            //------------------------------------------------
            XmlElement activityTime = doc.CreateElement("activityTime");
            investigationEvent.AppendChild(activityTime);
            //------------------------------------------------
            XmlElement availabilityTime = doc.CreateElement("availabilityTime");
            investigationEvent.AppendChild(availabilityTime);
            //------------------------------------------------
            XmlElement authorOrPerformer = doc.CreateElement("authorOrPerformer");
            authorOrPerformer.SetAttribute("typeCode", "AUT");

            XmlElement assignedEntity = doc.CreateElement("assignedEntity");
            authorOrPerformer.AppendChild(assignedEntity);

            investigationEvent.AppendChild(authorOrPerformer);
            //------------------------------------------------

            XmlElement trigger = GerarTriggerEReaction(doc);

            investigationEvent.AppendChild(trigger);
            //------------------------------------------------

            XmlElement pertinentInformations = doc.CreateElement("pertinentInformation1");
            XmlElement sequenceNumber = doc.CreateElement("sequenceNumber");
            sequenceNumber.SetAttribute("nullFlavor", "NI");
            pertinentInformations.AppendChild(sequenceNumber);
            //------------------------------------------------
            XmlElement secondaryCaseNotification = doc.CreateElement("secondaryCaseNotification");
            //------------------------------------------------
            XmlElement idSecondaryCaseNotification = doc.CreateElement("id");
            idSecondaryCaseNotification.SetAttribute("assigningAuthorityName", "FDA");
            idSecondaryCaseNotification.SetAttribute("extension", "NI");
            idSecondaryCaseNotification.SetAttribute("root", "2.16.840.1.113883.3.24");
            secondaryCaseNotification.AppendChild(idSecondaryCaseNotification);
            //------------------------------------------------
            XmlElement codeSCN = doc.CreateElement("code");
            codeSCN.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            codeSCN.SetAttribute("codeSystemName", "Type_of_Report");
            secondaryCaseNotification.AppendChild(codeSCN);
            //------------------------------------------------
            XmlElement effectiveTime = doc.CreateElement("effectiveTime");
            effectiveTime.SetAttribute("nullFlavor", "NI");
            secondaryCaseNotification.AppendChild(effectiveTime);
            //------------------------------------------------
            XmlElement receiver = doc.CreateElement("receiver");
            secondaryCaseNotification.AppendChild(receiver);
            XmlElement time = doc.CreateElement("time");
            time.SetAttribute("nullFlavor", "NI");
            receiver.AppendChild(time);
            XmlElement assignedEntitySCN = doc.CreateElement("assignedEntity");

            XmlElement assignedOrganizationSCN = doc.CreateElement("assignedOrganization");
            assignedEntitySCN.AppendChild(assignedOrganizationSCN);
            receiver.AppendChild(assignedEntitySCN);
            XmlElement codeAssOrg = doc.CreateElement("create");
            assignedOrganizationSCN.AppendChild(codeAssOrg);
            codeAssOrg.SetAttribute("code", "C17237");
            codeAssOrg.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            codeAssOrg.SetAttribute("codeSystemName", "Report_Receiver");
            //------------------------------------------------
            XmlElement authorSCN = doc.CreateElement("author");
            XmlElement assignedEntityAuthor = doc.CreateElement("assignedEntity");
            authorSCN.AppendChild(assignedEntityAuthor);
            XmlElement codeAEA = doc.CreateElement("code");
            assignedEntityAuthor.AppendChild(codeAEA);
            codeAEA.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            codeAEA.SetAttribute("codeSystemName", "Type_of_Reporter");

            secondaryCaseNotification.AppendChild(authorSCN);

            pertinentInformations.AppendChild(secondaryCaseNotification);
            investigationEvent.AppendChild(pertinentInformations);

            //------------------------------------------------
            XmlElement pertinentInfo1 = doc.CreateElement("pertinentInformation1");
            XmlElement SCN = doc.CreateElement("secondaryCaseNotifications");
            XmlElement idSCN = doc.CreateElement("id");
            SCN.AppendChild(idSCN);
            idSCN.SetAttribute("nullFlavor", "NA");
            XmlElement codeSCN2 = doc.CreateElement("code");
            SCN.AppendChild(codeSCN2);
            codeSCN2.SetAttribute("nullFlavor", "NI");
            XmlElement receiverSCN = doc.CreateElement("receiver");
            SCN.AppendChild(receiverSCN);
            XmlElement timeSCN = doc.CreateElement("time");
            receiverSCN.AppendChild(timeSCN);
            XmlElement low = doc.CreateElement("low");
            low.SetAttribute("nullFlavor", "NI");
            XmlElement high = doc.CreateElement("high");
            high.SetAttribute("value", "20240101");
            timeSCN.AppendChild(low);
            timeSCN.AppendChild(high);
            XmlElement assEnt = doc.CreateElement("assignedEntity");
            receiverSCN.AppendChild(assEnt);
            pertinentInfo1.AppendChild(SCN);
            investigationEvent.AppendChild(pertinentInfo1);

            XmlElement pertinentInformation2 = doc.CreateElement("pertinentInformation2");
            XmlElement caseSeriousness = doc.CreateElement("caseSeriousness");
            pertinentInformation2.AppendChild(caseSeriousness);
            XmlElement codeInfo2 = doc.CreateElement("code");
            codeInfo2.SetAttribute("code", "C49489");
            codeInfo2.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            codeInfo2.SetAttribute("codeSystemName", "Adverse_Event_Outcome");
            caseSeriousness.AppendChild(codeInfo2);
            XmlElement valueInfo2 = doc.CreateElement("value");
            valueInfo2.SetAttribute("code", "C52668");
            valueInfo2.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            valueInfo2.SetAttribute("xsi:type", "CE");
            caseSeriousness.AppendChild(valueInfo2);

            investigationEvent.AppendChild(pertinentInformation2);

            XmlElement pertainsTo = GerarPertainsTo(doc);

            investigationEvent.AppendChild(pertainsTo);

            XmlElement component = doc.CreateElement("component");
            XmlElement adverseEventAssessment = doc.CreateElement("adverseEventAssessment");
            XmlElement subject1 = doc.CreateElement("subject1");
            XmlElement primaryRole = doc.CreateElement("primaryRole");

            component.AppendChild(adverseEventAssessment);
            adverseEventAssessment.AppendChild(subject1);
            subject1.AppendChild(primaryRole);

            investigationEvent.AppendChild(component);

            return subject;
        }

        private static XmlElement GerarPertainsTo(XmlDocument doc)
        {
            XmlElement pertainsTo = doc.CreateElement("pertainsTo");
            XmlElement procedureEvent = doc.CreateElement("procedureEvent");
            pertainsTo.AppendChild(procedureEvent);
            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("nullFlavor", "ASKU");
            procedureEvent.AppendChild(code);
            //-----------------------------------------------------------
            XmlElement device = doc.CreateElement("device");
            procedureEvent.AppendChild(device);
            XmlElement identifiedDevice = doc.CreateElement("identifiedDevice");
            device.AppendChild(identifiedDevice);
            XmlElement id = doc.CreateElement("id");
            id.SetAttribute("nullFlavor", "NI");
            //-----------------------------------------------------------
            XmlElement identifiedDevice2 = doc.CreateElement("identifiedDevice");
            identifiedDevice.AppendChild(identifiedDevice2);
            XmlElement identifiedDevice2ID = doc.CreateElement("id");
            identifiedDevice2ID.SetAttribute("extension", "D4 SERIAL NUM");
            XmlElement existenceTime = doc.CreateElement("existenceTime");
            existenceTime.SetAttribute("value", "20240404");
            identifiedDevice2.AppendChild(identifiedDevice2ID);
            identifiedDevice2.AppendChild(existenceTime);
            XmlElement lotNumberText = doc.CreateElement("lotNumberText");
            lotNumberText.SetAttribute("mediaType", "text/plain");
            lotNumberText.InnerText = $"{Notificacao.LOTE}"; //"D4 LOT NUM";
            identifiedDevice2.AppendChild(lotNumberText);
            XmlElement expirationTime = doc.CreateElement("expirationTime");
            expirationTime.SetAttribute("value", "20240101");
            identifiedDevice2.AppendChild(expirationTime);
            //-----------------------------------------------------------
            XmlElement asManufacturedProduct = GerarAsManufacturedProduct(doc);

            identifiedDevice2.AppendChild(asManufacturedProduct);

            //-----------------------------------------------------------
            XmlElement inventoryItem = GerarInventoryItem(doc);

            identifiedDevice2.AppendChild(inventoryItem);

            //-----------------------------------------------------------
            GerarSubjectsOf(doc, ref identifiedDevice);

            //-----------------------------------------------------------
            XmlElement authorOrPerformer = doc.CreateElement("authorOrPerformer");
            XmlElement assignedEntity = doc.CreateElement("assignedEntity");
            XmlElement code2 = doc.CreateElement("code");
            XmlElement originalText = doc.CreateElement("originalText");
            authorOrPerformer.AppendChild(assignedEntity);
            assignedEntity.AppendChild(code2);
            code2.AppendChild(originalText);
            code.SetAttribute("code", "C53287");
            code.SetAttribute("codeSystemName", "Operator_of_Medical_Device");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            originalText.SetAttribute("mediaType", "text/plain");

            procedureEvent.AppendChild(authorOrPerformer);

            XmlElement pertinentInformation = doc.CreateElement("pertinentInformation1");
            XmlElement observation = doc.CreateElement("observation");
            XmlElement code3 = doc.CreateElement("code");
            XmlElement value = doc.CreateElement("value");

            pertinentInformation.AppendChild(observation);
            observation.AppendChild(code3);
            observation.AppendChild(value);

            observation.SetAttribute("moodCode", "EVN");
            code3.SetAttribute("code", "C53563");
            code3.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code3.SetAttribute("codeSystemName", "Single-Use_Device_Reprocessed_and_Reused_on_Patient");
            value.SetAttribute("xsi:type", "BL");
            value.SetAttribute("value", "false");

            procedureEvent.AppendChild(pertinentInformation);

            XmlElement pertinentInformation1 = doc.CreateElement("pertinentInformation1");
            XmlElement observation1 = doc.CreateElement("observation");
            XmlElement code4 = doc.CreateElement("code");
            XmlElement value1 = doc.CreateElement("value");

            pertinentInformation1.AppendChild(observation1);
            observation1.AppendChild(code4);
            observation1.AppendChild(value1);

            observation1.SetAttribute("moodCode", "EVN");
            code4.SetAttribute("code", "C85488");
            code4.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code4.SetAttribute("codeSystemName", "Device_Serviced_By_Third_Party");
            value1.SetAttribute("xsi:type", "BL");
            value1.SetAttribute("value", "false");

            procedureEvent.AppendChild(pertinentInformation1);

            XmlElement component1 = doc.CreateElement("component1");
            XmlElement implantation = doc.CreateElement("implantation");
            XmlElement effectiveTime = doc.CreateElement("effectiveTime");

            component1.AppendChild(implantation);
            implantation.AppendChild(effectiveTime);

            effectiveTime.SetAttribute("value", "20240606");

            procedureEvent.AppendChild(component1);

            XmlElement component2 = doc.CreateElement("component2");
            XmlElement explanation = doc.CreateElement("explanation");
            XmlElement effectiveTime1 = doc.CreateElement("effectiveTime");

            component2.AppendChild(explanation);
            explanation.AppendChild(effectiveTime1);

            effectiveTime1.SetAttribute("value", "20240606");
            procedureEvent.AppendChild(component2);


            //GerarSubjectsOf(doc, ref identifiedDevice);

            return pertainsTo;
        }

        private static void GerarSubjectsOf(XmlDocument doc, ref XmlElement identifiedDevice)
        {
            //-----------------------------------------------------------
            XmlElement subjectOf = doc.CreateElement("subjectOf");
            XmlElement deviceObservation = doc.CreateElement("deviceObservation");
            subjectOf.AppendChild(deviceObservation);
            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "C53449");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code.SetAttribute("codeSystemName", "Device_available_for_evaluation");
            deviceObservation.AppendChild(code);
            XmlElement effectiveTime = doc.CreateElement("effectiveTime");
            effectiveTime.SetAttribute("value", "20240909");
            deviceObservation.AppendChild(effectiveTime);
            XmlElement value = doc.CreateElement("value");
            value.SetAttribute("xsi:type", "BL");
            value.SetAttribute("value", "true");
            deviceObservation.AppendChild(value);

            identifiedDevice.AppendChild(subjectOf);
            //-----------------------------------------------------------
            XmlElement subjectOf1 = doc.CreateElement("subjectOf");
            XmlElement deviceObservation1 = doc.CreateElement("deviceObservation");
            subjectOf1.AppendChild(deviceObservation1);
            XmlElement code1 = doc.CreateElement("code");
            code1.SetAttribute("code", "C53451");
            code1.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code1.SetAttribute("codeSystemName", "Approximate_Age_of_Device");
            deviceObservation1.AppendChild(code1);
            XmlElement value1 = doc.CreateElement("value");
            value1.SetAttribute("xsi:type", "PQ");
            value1.SetAttribute("nullFlavor", "NI");
            deviceObservation1.AppendChild(value1);

            identifiedDevice.AppendChild(subjectOf1);
            //-----------------------------------------------------------
            XmlElement subjectOf2 = doc.CreateElement("subjectOf");
            XmlElement deviceObservation2 = doc.CreateElement("deviceObservation");
            subjectOf2.AppendChild(deviceObservation2);
            XmlElement code2 = doc.CreateElement("code");
            code2.SetAttribute("code", "C53982");
            code2.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code2.SetAttribute("codeSystemName", "Device_Problem_Code");
            deviceObservation2.AppendChild(code2);
            XmlElement value2 = doc.CreateElement("value");
            value2.SetAttribute("xsi:type", "CE");
            value2.SetAttribute("code", "3003");
            value2.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            deviceObservation2.AppendChild(value2);

            identifiedDevice.AppendChild(subjectOf2);
            //-----------------------------------------------------------
            XmlElement subjectOf3 = doc.CreateElement("subjectOf");
            XmlElement deviceObservation3 = doc.CreateElement("deviceObservation");
            subjectOf3.AppendChild(deviceObservation3);
            XmlElement code3 = doc.CreateElement("code");
            code3.SetAttribute("code", "C53629");
            code3.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code3.SetAttribute("codeSystemName", "Device_Evaluated_By_Manufacturer");
            deviceObservation3.AppendChild(code3);
            XmlElement value3 = doc.CreateElement("value");
            value3.SetAttribute("xsi:type", "BL");
            value3.SetAttribute("value", "true");
            deviceObservation3.AppendChild(value3);

            identifiedDevice.AppendChild(subjectOf3);
            //-----------------------------------------------------------
            XmlElement subjectOf4 = doc.CreateElement("subjectOf");
            XmlElement deviceObservation4 = doc.CreateElement("deviceObservation");
            subjectOf4.AppendChild(deviceObservation4);
            XmlElement code4 = doc.CreateElement("code");
            code4.SetAttribute("code", "C53591");
            code4.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code4.SetAttribute("codeSystemName", "Device_Returned_To_Manufacturer_For_Evaluation");
            deviceObservation4.AppendChild(code4);
            XmlElement value4 = doc.CreateElement("value");
            value4.SetAttribute("xsi:type", "BL");
            value4.SetAttribute("value", "true");
            deviceObservation4.AppendChild(value4);

            identifiedDevice.AppendChild(subjectOf4);
            //-----------------------------------------------------------
            XmlElement subjectOf5 = doc.CreateElement("subjectOf");
            XmlElement deviceObservation5 = doc.CreateElement("deviceObservation");
            subjectOf5.AppendChild(deviceObservation5);
            XmlElement code5 = doc.CreateElement("code");
            code5.SetAttribute("code", "C53592");
            code5.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code5.SetAttribute("codeSystemName", "Evaluation_Summary_Status");
            deviceObservation5.AppendChild(code5);
            XmlElement value5 = doc.CreateElement("value");
            value5.SetAttribute("xsi:type", "BL");
            value5.SetAttribute("value", "false");
            deviceObservation5.AppendChild(value5);

            identifiedDevice.AppendChild(subjectOf5);
            //-----------------------------------------------------------
            XmlElement subjectOf6 = doc.CreateElement("subjectOf");
            XmlElement deviceObservation6 = doc.CreateElement("deviceObservation");
            subjectOf6.AppendChild(deviceObservation6);
            XmlElement code6 = doc.CreateElement("code");
            code6.SetAttribute("code", "C53593");
            code6.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code6.SetAttribute("codeSystemName", "Reason_for_Non-Evaluation");
            deviceObservation6.AppendChild(code6);
            XmlElement value6 = doc.CreateElement("value");
            value6.SetAttribute("xsi:type", "CE");
            value6.SetAttribute("code", "NI");
            value6.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            deviceObservation6.AppendChild(value6);

            identifiedDevice.AppendChild(subjectOf6);

            //-----------------------------------------------------------
            XmlElement subjectOf7 = doc.CreateElement("subjectOf");
            XmlElement deviceObservation7 = doc.CreateElement("deviceObservation");
            subjectOf7.AppendChild(deviceObservation7);
            XmlElement code7 = doc.CreateElement("code");
            code7.SetAttribute("code", "C53602");
            code7.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code7.SetAttribute("codeSystemName", "Device_Labeled_for_single_use");
            deviceObservation7.AppendChild(code7);
            XmlElement value7 = doc.CreateElement("value");
            value7.SetAttribute("xsi:type", "BL");
            value7.SetAttribute("value", "true");
            deviceObservation7.AppendChild(value7);

            identifiedDevice.AppendChild(subjectOf7);

            //-----------------------------------------------------------
            XmlElement subjectOf8 = doc.CreateElement("subjectOf");
            XmlElement deviceObservation8 = doc.CreateElement("deviceObservation");
            subjectOf8.AppendChild(deviceObservation8);
            XmlElement code8 = doc.CreateElement("code");
            code8.SetAttribute("code", "C53603");
            code8.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code8.SetAttribute("codeSystemName", "Type_of_Remedial_Action");
            deviceObservation8.AppendChild(code8);
            XmlElement value8 = doc.CreateElement("value");
            value8.SetAttribute("xsi:type", "CE");
            value8.SetAttribute("code", "C53610");
            value8.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            deviceObservation8.AppendChild(value8);

            identifiedDevice.AppendChild(subjectOf8);

            //-----------------------------------------------------------
            XmlElement subjectOf9 = doc.CreateElement("subjectOf");
            XmlElement deviceObservation9 = doc.CreateElement("deviceObservation");
            subjectOf9.AppendChild(deviceObservation9);
            XmlElement code9 = doc.CreateElement("code");
            code9.SetAttribute("code", "C53645");
            code9.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code9.SetAttribute("codeSystemName", "Usage_of_Device");
            deviceObservation9.AppendChild(code9);
            XmlElement value9 = doc.CreateElement("value");
            value9.SetAttribute("xsi:type", "CE");
            value9.SetAttribute("code", "C53612");
            value9.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            deviceObservation9.AppendChild(value9);

            identifiedDevice.AppendChild(subjectOf9);

            //-----------------------------------------------------------
            XmlElement subjectOf10 = doc.CreateElement("subjectOf");
            XmlElement deviceObservation10 = doc.CreateElement("deviceObservation");
            subjectOf10.AppendChild(deviceObservation10);
            XmlElement code10 = doc.CreateElement("code");
            code10.SetAttribute("code", "C53619");
            code10.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code10.SetAttribute("codeSystemName", "Corrective_Action_Number");
            deviceObservation10.AppendChild(code10);
            XmlElement value10 = doc.CreateElement("value");
            value10.SetAttribute("xsi:type", "ED");
            value10.SetAttribute("mediaType", "text/plain");
            deviceObservation10.AppendChild(value10);

            identifiedDevice.AppendChild(subjectOf10);
        }

        private static XmlElement GerarInventoryItem(XmlDocument doc)
        {
            XmlElement inventoryItem = doc.CreateElement("inventoryItem");
            XmlElement manufacturedDeviceModel = doc.CreateElement("manufacturedDeviceModel");
            XmlElement id = doc.CreateElement("id");
            id.SetAttribute("extension", "@codigoProduto"); //TODO: Implementar variável
            manufacturedDeviceModel.AppendChild(id);
            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "DZE");
            code.SetAttribute("codeSystemName", "Type_of_Device");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            manufacturedDeviceModel.AppendChild(code);
            XmlElement originalText = doc.CreateElement("originalText");
            originalText.SetAttribute("mediaType", "text/plain");
            originalText.InnerText = "Endosseous Dental Implant";
            code.AppendChild(originalText);

            XmlElement manufacturerModelName = doc.CreateElement("manufacturerModelName");
            manufacturerModelName.SetAttribute("mediaType", "text/plain");
            manufacturerModelName.InnerText = $"Dental Implant {Notificacao.LINHA_DE_PRODUTO}"; //$"Dental Implant @LinhaProduto";
            manufacturedDeviceModel.AppendChild(manufacturerModelName);

            XmlElement asRegulatedProuct = doc.CreateElement("asRegulatedProduct");
            XmlElement id2 = doc.CreateElement("id");
            id2.SetAttribute("extension", "G4BLANUM");
            asRegulatedProuct.AppendChild(id2);
            manufacturedDeviceModel.AppendChild(asRegulatedProuct);

            XmlElement asRegulatedProuct2 = doc.CreateElement("asRegulatedProduct");
            XmlElement id3 = doc.CreateElement("id");
            id2.SetAttribute("extension", "G4COMBPROD");
            asRegulatedProuct.AppendChild(id3);
            manufacturedDeviceModel.AppendChild(asRegulatedProuct2);

            inventoryItem.AppendChild(manufacturedDeviceModel);

            return inventoryItem;
        }

        private static XmlElement GerarAsManufacturedProduct(XmlDocument doc)
        {
            XmlElement asManufacturedProduct = doc.CreateElement("asManufacturedProduct");
            XmlElement id = doc.CreateElement("id");
            id.SetAttribute("extension", "D4 CATALOG NUM");
            asManufacturedProduct.AppendChild(id);
            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "D4UDINUM");
            asManufacturedProduct.AppendChild(code);
            //----------------------------------------------------------------------------
            XmlElement manufacturerOrReprocessor = doc.CreateElement("manufacturerOrReprocessor");
            XmlElement code2 = doc.CreateElement("code");
            code2.SetAttribute("code", "C53616");
            code2.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code2.SetAttribute("codeSystemName", "Type_of_Manufacturer");
            manufacturerOrReprocessor.AppendChild(code2);
            XmlElement name = doc.CreateElement("name");
            name.InnerText = "S.I.N. Implant System";
            manufacturerOrReprocessor.AppendChild(name);

            XmlElement telecom = doc.CreateElement("telecom");
            telecom.SetAttribute("value", "fax:+05501121693000");
            manufacturerOrReprocessor.AppendChild(telecom);
            XmlElement telecom2 = doc.CreateElement("telecom");
            telecom2.SetAttribute("value", "mailto:anna.lopes@sinimplantsystem.com");
            manufacturerOrReprocessor.AppendChild(telecom2);
            //----------------------------------------------------------------------------
            XmlElement addr = doc.CreateElement("addr");
            manufacturerOrReprocessor.AppendChild(addr);
            XmlElement streetAddressLine = doc.CreateElement("streetAddressLine");
            streetAddressLine.InnerText = "R. Soldado Ocimar Guimaraes";
            addr.AppendChild(streetAddressLine);

            XmlElement streetAddressLine2 = doc.CreateElement("streetAddressLine");
            streetAddressLine2.InnerText = "da Silva, 421, Vila Rio Branco";
            addr.AppendChild(streetAddressLine2);

            XmlElement city = doc.CreateElement("city");
            city.InnerText = "Sao Paulo, SP";
            addr.AppendChild(city);
            XmlElement state = doc.CreateElement("state");
            addr.AppendChild(state);

            XmlElement postalCode = doc.CreateElement("postalCode");
            postalCode.InnerText = "03348-060";
            addr.AppendChild(postalCode);

            XmlElement country = doc.CreateElement("country");
            country.InnerText = "BRA";
            addr.AppendChild(country);

            asManufacturedProduct.AppendChild(manufacturerOrReprocessor);

            XmlElement asRole = doc.CreateElement("asRole");
            manufacturerOrReprocessor.AppendChild(asRole);
            XmlElement performance = doc.CreateElement("performance");
            asRole.AppendChild(performance);
            XmlElement investigationEvent = doc.CreateElement("investigationEvent");
            performance.AppendChild(investigationEvent);
            //----------------------------------------------------------------------------
            XmlElement contactParty = doc.CreateElement("contactParty");
            manufacturerOrReprocessor.AppendChild(contactParty);
            XmlElement addr2 = doc.CreateElement("addr");
            contactParty.AppendChild(addr2);
            XmlElement additionalLocator = doc.CreateElement("additionalLocator");
            additionalLocator.InnerText = "S.I.N. Implant System";
            addr2.AppendChild(additionalLocator);

            XmlElement streetAddressLine3 = doc.CreateElement("streetAddressLine");
            streetAddressLine3.InnerText = "R Soldado Ocimar Guimaraes da Silva, 421";
            addr2.AppendChild(streetAddressLine3);

            XmlElement streetAddressLine4 = doc.CreateElement("streetAddressLine");
            streetAddressLine4.InnerText = "Vila Rio Branco";
            addr2.AppendChild(streetAddressLine4);

            XmlElement city2 = doc.CreateElement("city");
            city2.InnerText = "Sao Paulo, SP";
            addr2.AppendChild(city2);

            XmlElement state2 = doc.CreateElement("state");
            addr2.AppendChild(state2);

            XmlElement postalCode2 = doc.CreateElement("postalCode");
            postalCode2.InnerText = "03348060";
            addr2.AppendChild(postalCode2);

            XmlElement country2 = doc.CreateElement("country");
            country2.InnerText = "BRA";
            addr2.AppendChild(country2);

            XmlElement telecom1 = doc.CreateElement("telecom");
            telecom1.SetAttribute("value", "");
            contactParty.AppendChild(telecom1);

            XmlElement telecom3 = doc.CreateElement("telecom");
            telecom3.SetAttribute("value", "mailto:anna.lopes@sinimplantsystem.com");
            contactParty.AppendChild(telecom3);

            XmlElement telecom4 = doc.CreateElement("telecom");
            telecom4.SetAttribute("value", "");
            contactParty.AppendChild(telecom4);

            XmlElement contactManufacturerContact = doc.CreateElement("contactManufacturerContact");
            XmlElement name2 = doc.CreateElement("name");
            contactManufacturerContact.AppendChild(name2);
            XmlElement prefix = doc.CreateElement("prefix");
            prefix.InnerText = "Ms";
            name2.AppendChild(prefix);

            XmlElement given = doc.CreateElement("given");
            given.InnerText = "Anna Paula";
            name2.AppendChild(given);

            XmlElement given2 = doc.CreateElement("given");
            given2.InnerText = "Rodrigues";
            name2.AppendChild(given2);

            XmlElement family = doc.CreateElement("family");
            family.InnerText = "Lopes";
            name2.AppendChild(family);


            contactParty.AppendChild(contactManufacturerContact);

            return asManufacturedProduct;
        }

        private static XmlElement GerarTriggerEReaction(XmlDocument doc)
        {
            XmlElement trigger = doc.CreateElement("trigger");
            XmlElement reaction = doc.CreateElement("reaction");

            trigger.AppendChild(reaction);
            //-----------------------------------------------------------
            XmlElement text = doc.CreateElement("text");
            text.SetAttribute("mediaType", "text/plain");
            text.InnerText = "The clinician reports that the dental implant failed to osseointegrate. The device was returned to the manufacturer.";

            reaction.AppendChild(text);
            //-----------------------------------------------------------
            XmlElement term = doc.CreateElement("term");
            term.SetAttribute("mediaType", "text/plain");
            term.InnerText = "";

            reaction.AppendChild(term);
            //-----------------------------------------------------------
            XmlElement effectiveTime = doc.CreateElement("effectiveTime");
            effectiveTime.SetAttribute("value", "20240830");

            reaction.AppendChild(effectiveTime);
            //-----------------------------------------------------------
            XmlElement subject = doc.CreateElement("subject");
            XmlElement investigativeSubject = doc.CreateElement("investigativeSubject");
            subject.AppendChild(investigativeSubject);
            //-----------------------------------------------------------
            XmlElement subjectAffectedPerson = doc.CreateElement("subjectAffectedPerson");
            investigativeSubject.AppendChild(subjectAffectedPerson);
            //-----------------------------------------------------------
            XmlElement name = doc.CreateElement("name");
            name.InnerText = "T.E.S.T.E.";
            subjectAffectedPerson.AppendChild(name);
            //-----------------------------------------------------------
            XmlElement administrativeGenderCode = doc.CreateElement("administrativeGenderCode");
            administrativeGenderCode.SetAttribute("code", "C20197");
            administrativeGenderCode.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            administrativeGenderCode.SetAttribute("codeSystemName", "Sex");
            subjectAffectedPerson.AppendChild(administrativeGenderCode);
            //-----------------------------------------------------------
            XmlElement birthTime = doc.CreateElement("birthTime");
            birthTime.SetAttribute("value", "20000101");
            subjectAffectedPerson.AppendChild(birthTime);
            //-----------------------------------------------------------
            XmlElement deceasedTime = doc.CreateElement("deceasedTime");
            deceasedTime.SetAttribute("nullFlavor", "NI");
            subjectAffectedPerson.AppendChild(deceasedTime);

            reaction.AppendChild(subject);

            XmlElement subjectOfIdade = GerarSubjectOfIdade(doc);
            investigativeSubject.AppendChild(subjectOfIdade);

            XmlElement subjectOfPeso = GerarSubjectOfPeso(doc);
            investigativeSubject.AppendChild(subjectOfPeso);

            XmlElement subjectOfGenero = GerarSubjectOfGenero(doc);
            investigativeSubject.AppendChild(subjectOfGenero);

            XmlElement subjectOfInformacaoRelevante = GerarSubjectOfInformacaoRelevante(doc);
            investigativeSubject.AppendChild(subjectOfInformacaoRelevante);

            XmlElement subjectOfInformacaoRelevante2 = GerarSubjectOfOutraInformacaoRelevante(doc);
            investigativeSubject.AppendChild(subjectOfInformacaoRelevante2);

            XmlElement subjectOfInformacaoProblemaDoPaciente = GerarSubjectOfProblemaDoPaciente(doc);
            investigativeSubject.AppendChild(subjectOfInformacaoProblemaDoPaciente);

            //TODO: Dentro de Reaction, Adicionar as Pertinent Information
            XmlElement pertinentInformation = doc.CreateElement("pertinentInformation");
            reaction.AppendChild(pertinentInformation);
            //-----------------------------------------------------------
            XmlElement primarySourceReport = GerarPrimarySourceReport(doc);
            pertinentInformation.AppendChild(primarySourceReport);

            return trigger;
        }

        private static XmlElement GerarPrimarySourceReport(XmlDocument doc)
        {
            XmlElement primarySourceReport = doc.CreateElement("primarySourceReport");
            XmlElement id = doc.CreateElement("id");
            id.SetAttribute("nullFlavor", "ASKU");
            primarySourceReport.AppendChild(id);
            //-----------------------------------------------------------
            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("nullFlavor", "ASKU");
            primarySourceReport.AppendChild(code);
            //-----------------------------------------------------------
            XmlElement receiver = doc.CreateElement("receiver");
            receiver.SetAttribute("negationInd", "true");
            primarySourceReport.AppendChild(receiver);

            XmlElement assignedEntiy = doc.CreateElement("assignedEntity");
            receiver.AppendChild(assignedEntiy);
            XmlElement assignedOrganization = doc.CreateElement("assignedOrganization");
            assignedEntiy.AppendChild(assignedOrganization);
            XmlElement name = doc.CreateElement("name");
            name.InnerText = "FDA";
            assignedOrganization.AppendChild(name);
            //-----------------------------------------------------------

            XmlElement author = GerarAuthor(doc);
            primarySourceReport.AppendChild(author);


            return primarySourceReport;
        }

        private static XmlElement GerarAuthor(XmlDocument doc)
        {
            XmlElement author = doc.CreateElement("author");
            XmlElement assignedEntity = doc.CreateElement("assignedEntity");
            author.AppendChild(assignedEntity);

            XmlElement code = doc.CreateElement("code");
            assignedEntity.AppendChild(code);
            code.SetAttribute("code", "C52654");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code.SetAttribute("codeSystemName", "Occupation");
            XmlElement originalText = doc.CreateElement("originalText");
            code.SetAttribute("mediaType", "text/plain");
            code.AppendChild(originalText);
            //-----------------------------------------------------------
            XmlElement assignedPerson = doc.CreateElement("assignedPerson");
            assignedEntity.AppendChild(assignedPerson);
            XmlElement name = doc.CreateElement("name");
            XmlElement prefix = doc.CreateElement("prefix");
            prefix.InnerText = "Mr."; //"@Titulo_Pessoa_Report"; //TODO: VARIAVEIS A SUBSTITUIR
            name.AppendChild(prefix);
            XmlElement given1 = doc.CreateElement("given");
            given1.InnerText = Notificacao.CLIENTE; //"@Nome_Cliente";
            name.AppendChild(given1);
            XmlElement given2 = doc.CreateElement("given");
            given2.InnerText = Notificacao.CLIENTE; //"@Nome_Do_Meio_Cliente";
            name.AppendChild(given2);

            XmlElement family = doc.CreateElement("family");
            name.AppendChild(family);
            family.InnerText = Notificacao.CLIENTE; //"@Nome_Ultimo_Cliente";

            assignedPerson.AppendChild(name);
            //name.AppendChild(prefix);
            //name.AppendChild(given);
            //name.AppendChild(given);

            //-----------------------------------------------------------
            XmlElement telecom1 = doc.CreateElement("telecom");
            telecom1.SetAttribute("value", "");
            assignedPerson.AppendChild(telecom1);

            XmlElement telecom2 = doc.CreateElement("telecom");
            telecom2.SetAttribute("value", "mailto:E1EMAIL@GMAIL.COM");
            assignedPerson.AppendChild(telecom2);

            XmlElement telecom3 = doc.CreateElement("telecom");
            telecom3.SetAttribute("value", "fax:+1100011010011011");
            assignedPerson.AppendChild(telecom3);
            //-----------------------------------------------------------
            XmlElement addr = doc.CreateElement("addr");
            assignedPerson.AppendChild(addr);

            XmlElement streetAddressLine = doc.CreateElement("streetAddressLine");
            addr.AppendChild(streetAddressLine);
            streetAddressLine.InnerText = Notificacao.enderecoCliente; //"@Rua_Cliente";

            XmlElement streetAddressLine2 = doc.CreateElement("streetAddressLine");
            addr.AppendChild(streetAddressLine2);
            streetAddressLine2.InnerText = Notificacao.enderecoCliente; //"@Rua_Cliente2";

            XmlElement city = doc.CreateElement("city");
            addr.AppendChild(city);
            city.InnerText = Notificacao.cidadeCliente; //"@Cidade_Cliente";

            XmlElement state = doc.CreateElement("state");
            addr.AppendChild(state);
            //state.InnerText = "@Cidade_Cliente";

            XmlElement postalCode = doc.CreateElement("postalCode");
            addr.AppendChild(postalCode);
            postalCode.InnerText = Notificacao.cepCliente; //"@CEP_Cliente";

            XmlElement country = doc.CreateElement("country");
            addr.AppendChild(country);
            country.InnerText = "BRA";
            //-----------------------------------------------------------
            XmlElement representedOrganization = doc.CreateElement("representedOrganization");
            assignedEntity.AppendChild(representedOrganization);
            XmlElement nameOrg = doc.CreateElement("name");
            nameOrg.InnerText = Notificacao.nomeRede; //"@NomeEstabelecimento";
            representedOrganization.AppendChild(nameOrg);

            return author;
        }

        private static XmlElement GerarSubjectOfProblemaDoPaciente(XmlDocument doc)
        {
            XmlElement subjectOf = doc.CreateElement("subjectOf");
            XmlElement observation = doc.CreateElement("observation");
            observation.SetAttribute("moodCode", "EVN");

            subjectOf.AppendChild(observation);

            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "C53983");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code.SetAttribute("codeSystemName", "Patient_Problem_Code");
            subjectOf.AppendChild(code);

            XmlElement value = doc.CreateElement("value");
            value.SetAttribute("xsi:type", "CE");
            value.SetAttribute("code", "4582");
            value.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");

            //value.InnerText = "This is just a test to generate a HL7 XML file. This is the form's section B7.";
            subjectOf.AppendChild(value);


            return subjectOf;
        }

        private static XmlElement GerarSubjectOfOutraInformacaoRelevante(XmlDocument doc)
        {
            XmlElement subjectOf = doc.CreateElement("subjectOf");
            XmlElement observation = doc.CreateElement("observation");
            observation.SetAttribute("moodCode", "EVN");

            subjectOf.AppendChild(observation);

            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "C53263");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code.SetAttribute("codeSystemName", "Other_Personal_Medical_History");
            subjectOf.AppendChild(code);

            XmlElement value = doc.CreateElement("value");
            value.SetAttribute("mediaType", "text/plain");
            value.SetAttribute("xsi:type", "ED");
            //value.InnerText = "This is just a test to generate a HL7 XML file. This is the form's section B7.";
            subjectOf.AppendChild(value);


            return subjectOf;
        }

        private static XmlElement GerarSubjectOfInformacaoRelevante(XmlDocument doc)
        {
            XmlElement subjectOf = doc.CreateElement("subjectOf");
            XmlElement observation = doc.CreateElement("observation");
            observation.SetAttribute("moodCode", "EVN");

            subjectOf.AppendChild(observation);

            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "C36292");
            code.SetAttribute("codeSystem", "NCI");
            code.SetAttribute("codeSystemName", "Test_Result");
            subjectOf.AppendChild(code);

            XmlElement value = doc.CreateElement("value");
            value.SetAttribute("mediaType", "text/plain");
            value.SetAttribute("xsi:type", "ED");
            //value.InnerText = "This is just a test to generate a HL7 XML file. This is the form's section B6.";
            subjectOf.AppendChild(value);


            return subjectOf;
        }

        private static XmlElement GerarSubjectOfGenero(XmlDocument doc)
        {
            XmlElement subjectOf = doc.CreateElement("subjectOf");
            XmlElement observation = doc.CreateElement("observation");
            observation.SetAttribute("moodCode", "EVN");

            subjectOf.AppendChild(observation);

            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "C17357");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code.SetAttribute("codeSystemName", "GenderType");
            subjectOf.AppendChild(code);

            XmlElement value = doc.CreateElement("value");
            value.SetAttribute("xsi:type", "CE");
            value.SetAttribute("code", "C46109");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            subjectOf.AppendChild(value);


            return subjectOf;
        }

        private static XmlElement GerarSubjectOfPeso(XmlDocument doc)
        {
            XmlElement subjectOf = doc.CreateElement("subjectOf");
            XmlElement observation = doc.CreateElement("observation");
            observation.SetAttribute("moodCode", "EVN");

            subjectOf.AppendChild(observation);

            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "C25208");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code.SetAttribute("codeSystemName", "Weight");
            subjectOf.AppendChild(code);

            XmlElement value = doc.CreateElement("value");
            value.SetAttribute("xsi:type", "PQ");
            value.SetAttribute("unit", "kg");
            subjectOf.AppendChild(value);


            return subjectOf;
        }

        private static XmlElement GerarSubjectOfIdade(XmlDocument doc)
        {
            XmlElement subjectOf = doc.CreateElement("subjectOf");
            XmlElement observation = doc.CreateElement("observation");
            observation.SetAttribute("moodCode", "EVN");

            subjectOf.AppendChild(observation);

            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "C25150");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code.SetAttribute("codeSystemName", "age");
            subjectOf.AppendChild(code);

            XmlElement value = doc.CreateElement("value");
            value.SetAttribute("xsi:type", "PQ");
            value.SetAttribute("unit", "");
            subjectOf.AppendChild(value);


            return subjectOf;
        }

        private static XmlElement GerarAuthorOrPerformer(XmlDocument doc)
        {
            XmlElement authorOrPerformer = doc.CreateElement("authorOrPerformer");
            XmlElement assignedPerson = doc.CreateElement("assignedPerson");
            XmlElement code = doc.CreateElement("code");
            code.SetAttribute("code", "C53616");
            code.SetAttribute("codeSystem", "2.16.840.1.113883.3.26.1.1");
            code.SetAttribute("codeSystemName", "Type_of_Reporter");

            assignedPerson.AppendChild(code);
            authorOrPerformer.AppendChild(assignedPerson);

            return authorOrPerformer;
        }

        private static XmlElement GerarSenderDoMessage(XmlDocument doc)
        {
            XmlElement sender = doc.CreateElement("sender");
            XmlElement telecom = doc.CreateElement("telecom");
            XmlElement device = doc.CreateElement("device");
            XmlElement id = doc.CreateElement("id");
            id.SetAttribute("nullFlavor", "NA");

            sender.AppendChild(telecom);

            device.AppendChild(id);

            sender.AppendChild(device);


            return sender;
        }

        private static XmlElement GerarReceiverDoMessage(XmlDocument doc)
        {
            XmlElement receiver = doc.CreateElement("receiver");
            XmlElement telecom = doc.CreateElement("telecom");
            XmlElement device = doc.CreateElement("device");
            XmlElement id = doc.CreateElement("id");
            id.SetAttribute("nullFlavor", "NA");

            receiver.AppendChild(telecom);

            device.AppendChild(id);

            receiver.AppendChild(device);


            return receiver;
        }

        private static XmlElement GerarSenderXMLFDA(XmlDocument doc, XmlElement idRepresentedOrganization, XmlElement nameRepresentedOrganization)
        {
            #region Sender do arquivo XML para o FDA
            XmlElement sender = doc.CreateElement("sender");
            XmlElement telecomSender = doc.CreateElement("telecom");

            sender.AppendChild(telecomSender);

            XmlElement deviceSender = doc.CreateElement("device");
            XmlElement deviceSenderId = doc.CreateElement("id");
            deviceSenderId.SetAttribute("nullFlavor", "NA");
            deviceSender.AppendChild(deviceSenderId);
            XmlElement deviceSoftwareName = doc.CreateElement("softwareName");
            deviceSoftwareName.InnerText = "emdr_esub";
            deviceSender.AppendChild(deviceSoftwareName);

            //Cria a Tag asAgent que fica dentro de Receiver
            XmlElement asAgentSender = doc.CreateElement("asAgent");
            //Cria a Tag representedOrganization que fica dentro de asAgent
            XmlElement representedOrganizationSender = doc.CreateElement("representedOrganization");
            //Cria a Tag representedOrganization que fica dentro de asAgent
            XmlElement idRepresentedOrganizationSender = doc.CreateElement("id");
            idRepresentedOrganizationSender.SetAttribute("nullFlavor", "NA");
            XmlElement nameRepresentedOrganizationSender = doc.CreateElement("name");
            nameRepresentedOrganizationSender.InnerText = "USA Device Manufacturer";
            deviceSender.AppendChild(asAgentSender);
            asAgentSender.AppendChild(representedOrganizationSender);
            representedOrganizationSender.AppendChild(idRepresentedOrganizationSender);
            representedOrganizationSender.AppendChild(nameRepresentedOrganizationSender);

            sender.AppendChild(deviceSender);

            #endregion
            return sender;
        }

        private static void GerarReceiverDoXMLFDA(XmlDocument doc, out XmlElement receiverElement, out XmlElement idRepresentedOrganization, out XmlElement nameRepresentedOrganization)
        {
            #region Receiver do XML para FDA
            receiverElement = doc.CreateElement("receiver");
            XmlElement telecom = doc.CreateElement("telecom");
            receiverElement.AppendChild(telecom);

            XmlElement device = doc.CreateElement("device");
            XmlElement deviceId = doc.CreateElement("id");
            deviceId.SetAttribute("nullFlavor", "NA");
            device.AppendChild(deviceId);
            receiverElement.AppendChild(device);

            //Cria a Tag asAgent que fica dentro de Receiver
            XmlElement asAgent = doc.CreateElement("asAgent");
            //Cria a Tag representedOrganization que fica dentro de asAgent
            XmlElement representedOrganization = doc.CreateElement("representedOrganization");
            //Cria a Tag representedOrganization que fica dentro de asAgent
            idRepresentedOrganization = doc.CreateElement("id");
            idRepresentedOrganization.SetAttribute("nullFlavor", "NA");
            /**************************************************************/
            nameRepresentedOrganization = doc.CreateElement("name");
            nameRepresentedOrganization.InnerText = "CDHR";
            device.AppendChild(asAgent);
            asAgent.AppendChild(representedOrganization);
            representedOrganization.AppendChild(idRepresentedOrganization);
            representedOrganization.AppendChild(nameRepresentedOrganization);
            #endregion
        }
    }
}
