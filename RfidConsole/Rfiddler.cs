using System;
using System.Runtime.InteropServices;
using rfid;
using rfid.Constants;
using rfid.Structures;
using Serilog;

namespace RfidConsole
{
    public class Rfiddler
    {
        private static ILogger logger = Log.Logger.ForContext<Rfiddler>();

        static Int32 callbackCount;
        static uint enableSelectCriteria = 0x00; // set to 0x01 to enable
        static uint enablePostSingulationMatch = 0x00; // set to 0x02 to enable
        static uint enableTagFocus = 0x00; // set to 0x01 to enable
        static uint enableTagSuppression = 0x00; // set to 0x01 to enable
        static uint enableFastId = 0x00; // set to 0x01 to enable
        private bool enableTagLock = false;
        private bool enableTagKill = false;

        public Int32 MyCallback
        (
            [In] Int32 handle,
            [In] UInt32 bufferLength,
            [In] IntPtr pBuffer,
            [In, Out] IntPtr context
        )
        {
            Byte[] packetBuffer = new Byte[bufferLength];

            Marshal.Copy(pBuffer, packetBuffer, 0, (Int32) bufferLength);

            var packetFlags = packetBuffer[1];
            var packetType = (Int16) ((packetBuffer[3] << 8) | packetBuffer[2]);
            var packetLength = (Int16) ((packetBuffer[5] << 8) | packetBuffer[4]);
            var packetTypeString = "Mac Packet rcv'd, PacketType = ";
            packetTypeString += $"0x{packetType:X4}";

            logger.Information(packetTypeString);

            // if its an end packet, print the status string too.
            if (1 == packetType)
            {
                var packetStatusString = "EndPacket Status = ";
                packetStatusString += string.Format("0x{0:X2}{1:X2}{2:X2}{3:X2}", packetBuffer[15], packetBuffer[14], packetBuffer[13], packetBuffer[12]);

                logger.Information(packetStatusString);
            }
            else if (5 == packetType)
            {
                Int16 length = (Int16) (((packetLength - 3) * 4) - (packetFlags >> 6));
                string packetEpcString = "    EPC = ";
                for (int index = 0; index < length; ++index)
                {
                    packetEpcString += $"{packetBuffer[20 + index]:X2}";
                }

                // logger.Verbose(packetEpcString);
            }
            else if (6 == packetType) // access packet, print the flag word if non-zero, along with error indicators
            {
                Byte flagWord = packetBuffer[1];
                var packetFlagString = "AccessPacket Flag = ";
                packetFlagString += string.Format("0x{0:X2}", flagWord);

                if (0 != (flagWord & 0x03)) // some error occurred
                {
                    if (0 != (flagWord & 0x0E)) // backscatter error, reported in the tag_error_code
                    {
                        packetFlagString += ", Error = " + string.Format("0x{0:X2}", packetBuffer[13]);
                    }
                    else // MAC protocol access error, reported prot_error_code field
                    {
                        packetFlagString += ", Error = " + string.Format("0x{0:X2}{1:X2}", packetBuffer[15], packetBuffer[14]);
                    }
                    logger.Information(packetFlagString);
                }
            }

            ++callbackCount;

            if (40 == callbackCount)
            {
                return 1;
            }

            return 0;
        }

        public static void Start(string[] args)
        {
            Linkage link = new Linkage();

            ShowLinkageLibraryVersion(link);

            RadioEnumeration re = EnumerateRadios(link);

            Int32 radioHandle = 0;

            radioHandle = OpenRadio(link, re, radioHandle);

            RadioClose(link, radioHandle);

            radioHandle = OpenRadio(link, re, radioHandle);

            GetRadioOperationMode(link, radioHandle);

            GetMacVersion(link, radioHandle);

            GetMacBootloaderVersion(link, radioHandle);

            SetRadioOperationMode(link, radioHandle, RadioOperationMode.NONCONTINUOUS);

            GetRadioOperationMode(link, radioHandle);

            GetRadioPowerState(link, radioHandle);

            SetRadioPowerState(link, radioHandle, RadioPowerState.FULL);

            GetRadioPowerState(link, radioHandle);

            SetRadioPowerState(link, radioHandle, RadioPowerState.STANDBY);

            GetRadioPowerState(link, radioHandle);

            EnumerateLinkProfiles(link, radioHandle);

            GetAntennaPortStatus(link, radioHandle);

            SetAntennaPortStatus(link, radioHandle, AntennaPortState.DISABLED);

            GetAntennaPortStatus(link, radioHandle);
            
            SetAntennaPortStatus(link, radioHandle, AntennaPortState.ENABLED);

            GetAntennaPortStatus(link, radioHandle);

            AntennaPortGetConfiguration(link, radioHandle);

            AntennaPortSetConfiguration();
            
            var selectCriteria = Set18K6CSelectCriteria(link, radioHandle);

            Get18K6CSelectCriteria(link, radioHandle, selectCriteria);

            Set18K6CQueryTagGroup(link, radioHandle);

            UseImpinjExtensions(link, radioHandle);

            Set18K6CPostMatchCriteria(link, radioHandle);

            Set18K6CSingulationAlgorithmParameters(link, radioHandle);

            //result = link.RadioSetResponseDataMode( radioHandle, ResponseType.DATA, ResponseMode.EXTENDED );

            //logger.Information(string.Empty);
            //logger.Information(string.Empty);
            //logger.Information( "link.RadioSetResponseDataMode result : " + result );

            Tag18K6CInventory(link, radioHandle);

            Tag18K6CRead(link, radioHandle);

            Tag18K6CQT(link, radioHandle);

            Tag18K6CBlockErase(link, radioHandle);

            Tag18K6CKill(link, radioHandle);

            Tag18K6CLock(link, radioHandle);

            RadioTurnCarrierWaveOnRandom(link, radioHandle);

            RegisterAccess(link, radioHandle);

            RadioClose(link, radioHandle);

            link.Shutdown();
        }

        private static void RegisterAccess(Linkage link, int radioHandle)
        {
            // test a banked register write            
            var result = link.MacWriteBankedRegister(radioHandle, 0x704, 2, 0xA5A5);
            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacWriteBankedRegister result : " + result);

            // should give an error, due to invalid selector
            result = link.MacWriteBankedRegister(radioHandle, 0x704, 16, 0xA5A5);
            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacWriteBankedRegister result (Expect Invalid Parameter) : " + result);

            logger.Information(string.Empty);
            logger.Information(string.Empty);


            // test a banked register read, last iteration i=16 should give an error
            UInt32 value = 0;
            for (UInt16 i = 0; i < 17; i++)
            {
                result = link.MacReadBankedRegister(radioHandle, 0x704, i, ref value);
                if (i < 16)
                {
                    logger.Information("link.MacReadBankedRegister result : " + result);
                }
                else
                {
                    logger.Information("link.MacReadBankedRegister result (Expect Invalid Parameter) : " + result);
                }
            }

            // Test OEMConfig Read/Write            
            UInt32[] readData = new UInt32[20];
            UInt32 numToRead = 16;
            result = link.MacReadOemData(radioHandle, 0x1f70, ref numToRead, readData);
            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacReadOemData result : " + result + ", numRead=" + numToRead);

            numToRead++;
            result = link.MacReadOemData(radioHandle, 0x1f70, ref numToRead, readData);
            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacReadOemData result (Expect Invalid Parameter) : " + result + ", numRead=" + numToRead);

            UInt32 numToWrite = 16;
            result = link.MacWriteOemData(radioHandle, 0x1f70, ref numToWrite, readData);
            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacWriteOemData result : " + result + ", numWritten=" + numToWrite);

            numToWrite++;
            result = link.MacWriteOemData(radioHandle, 0x1f70, ref numToWrite, readData);
            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacWriteOemData result (Expect Invalid Parameter) : " + result + ", numWritten=" + numToWrite);


            // force a mac error writing an invalid address and confirm mac error and macLastError
            // can be retrieved and reflect the same value
            result = link.MacWriteRegister(radioHandle, 0xFFFF, 0);
            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacWriteRegister result (Expect Invalid Parameter) : " + result);
            UInt32 macError = 0;
            UInt32 macLastError = 0;
            result = link.MacGetError(radioHandle, ref macError, ref macLastError);
            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacGetError (" + macError + ", " + macLastError + ") result : " + result);

            // confirm a successful register access clears the mac error but not the lastMacError
            UInt32 macVer = 0;
            result = link.MacReadRegister(radioHandle, 0x0000, ref macVer);
            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacReadRegister result : " + result);
            result = link.MacGetError(radioHandle, ref macError, ref macLastError);
            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacGetError (" + macError + ", " + macLastError + ") result : " + result);
        }

        private static void RadioTurnCarrierWaveOnRandom(Linkage link, int radioHandle)
        {
            var program = new Rfiddler();

            RandomCwParms randomCwParms = new RandomCwParms();

            // Enable Tx Random Data status packet by setting bits 7 and 0 in HST_CMNDIAGS (0x201)           
            UInt32 hstCmnDiags = 0;
            var result = link.MacReadRegister(radioHandle, 0x0201, ref hstCmnDiags);
            logger.Information(string.Empty);
            logger.Information("link.MacReadRegister result : " + result);
            hstCmnDiags |= 0x81;
            result = link.MacWriteRegister(radioHandle, 0x0201, hstCmnDiags);
            logger.Information(string.Empty);
            logger.Information("link.MacWriteRegister result : " + result);
            logger.Information(string.Empty);

            logger.Information("Starting random cw test:");
            logger.Information("Limiting to 40 MAC packets");

#if TEST_NULL_RANDOM_CW_CALLBACK
            randomCwParms.callback = null;
            randomCwParms.context = (IntPtr) null;
            randomCwParms.callbackCode = (IntPtr) null;
#else
            randomCwParms.callback = program.MyCallback;
            randomCwParms.context = IntPtr.Zero;
            randomCwParms.callbackCode = IntPtr.Zero;
#endif
            randomCwParms.duration = 10000;

            result = link.RadioTurnCarrierWaveOnRandom(radioHandle, randomCwParms);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.RadioTurnCarrierWaveOnRandom result : " + result);
        }

        private static void Tag18K6CLock(Linkage link, int radioHandle)
        {
            var program = new Rfiddler();

            // LOCK check - do not want to accidentally have tags locked unexpectedly...
            if (!program.enableTagLock)
            {
                return;
            }

            LockParms lockParms = new LockParms();

            lockParms.common.tagStopCount = 0;
            lockParms.common.callback = program.MyCallback;
            lockParms.common.context = IntPtr.Zero;
            lockParms.common.callbackCode = IntPtr.Zero;

            lockParms.lockCmdParms.permissions.killPasswordPermissions = PasswordPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.accessPasswordPermissions = PasswordPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.epcMemoryBankPermissions = MemoryPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.tidMemoryBankPermissions = MemoryPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.userMemoryBankPermissions = MemoryPermission.NO_CHANGE;

            lockParms.accessPassword = 0x0;

            var result = link.Tag18K6CLock(radioHandle, lockParms, 0);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Tag18K6CLock result : " + result);
        }

        private static void Tag18K6CKill(Linkage link, int radioHandle)
        {
            var program = new Rfiddler();

            // KILL check - do not want to accidentally have tags killed unexpectedly...
            if (!program.enableTagKill)
            {
                return;
            }

            KillParms killParms = new KillParms();

            killParms.common.tagStopCount = 0;
            killParms.common.callback = program.MyCallback;
            killParms.common.context = IntPtr.Zero;
            killParms.common.callbackCode = IntPtr.Zero;

            killParms.accessPassword = 0x0;
            killParms.killCmdParms.killPassword = 0x0;

            var result = link.Tag18K6CKill(radioHandle, killParms, 0);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Tag18K6CKill result : " + result);
        }

        private static void Tag18K6CBlockErase(Linkage link, int radioHandle)
        {
            var program = new Rfiddler();

            BlockEraseParms blockEraseParms = new BlockEraseParms();

            logger.Information("Starting basic block erase test:");
            logger.Information("Limiting of 40 MAC packets");

            blockEraseParms.common.tagStopCount = 0;
            blockEraseParms.common.callback = program.MyCallback;
            blockEraseParms.common.context = IntPtr.Zero;
            blockEraseParms.common.callbackCode = IntPtr.Zero;

            blockEraseParms.blockEraseCmdParms.bank = MemoryBank.RESERVED;
            blockEraseParms.blockEraseCmdParms.offset = 0;
            blockEraseParms.blockEraseCmdParms.count = 4;

            blockEraseParms.accessPassword = 0x0;

            var result = link.Tag18K6CBlockErase(radioHandle, blockEraseParms, 0);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Tag18K6CBlockErase result : " + result);
        }

        private static void Tag18K6CQT(Linkage link, int radioHandle)
        {
            var program = new Rfiddler();

            QTParms qtParms = new QTParms();


            logger.Information("Starting basic QT test:");
            logger.Information("Limiting of 40 MAC packets");

            qtParms.common.tagStopCount = 0;
            qtParms.common.callback = program.MyCallback;
            qtParms.common.context = IntPtr.Zero;
            qtParms.common.callbackCode = IntPtr.Zero;

            qtParms.optCmdType = OptType.OPT_READ;

            ReadCmdParms readCmdParms = new ReadCmdParms();

            readCmdParms.bank = MemoryBank.EPC;
            readCmdParms.offset = 2;
            readCmdParms.count = 6;

            qtParms.accessParms = readCmdParms;

            qtParms.qtCmdParms.qtMemoryMap = QTMemMapType.PRIVATE;
            qtParms.qtCmdParms.qtPersistence = QTPersistenceType.PERMANENT;
            qtParms.qtCmdParms.qtReadWrite = QTCtrlType.READ;
            qtParms.qtCmdParms.qtShortRange = QTShortRangeType.DISABLE;

            qtParms.accessPassword = 0x0;

            var result = link.Tag18K6CQT(radioHandle, qtParms, 0);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Tag18K6CQT result : " + result);
        }

        private static void Tag18K6CRead(Linkage link, int radioHandle)
        {
            Rfiddler program = new Rfiddler();

            ReadParms readParms = new ReadParms();

            logger.Information("Starting basic read test:");
            logger.Information("Limiting of 40 MAC packets");

            readParms.common.tagStopCount = 0;
            readParms.common.callback = program.MyCallback;
            readParms.common.context = IntPtr.Zero;
            readParms.common.callbackCode = IntPtr.Zero;

            readParms.readCmdParms.bank = MemoryBank.EPC;
            readParms.readCmdParms.offset = 2;
            readParms.readCmdParms.count = 6;

            readParms.accessPassword = 0x0;

            var result = link.Tag18K6CRead(radioHandle, readParms, 0);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Tag18K6CRead result : " + result);
        }

        private static void Tag18K6CInventory(Linkage link, int radioHandle)
        {
            logger.Information("Starting basic inventory test:");
            logger.Information("Limiting of 40 MAC packets");


            Rfiddler program = new Rfiddler();

            InventoryParms inventoryParms = new InventoryParms();

            inventoryParms.common = new CommonParms();

            inventoryParms.common.tagStopCount = 0;
            inventoryParms.common.callback = program.MyCallback;
            inventoryParms.common.context = IntPtr.Zero;
            inventoryParms.common.callbackCode = IntPtr.Zero;

            var result = link.Tag18K6CInventory(radioHandle, inventoryParms, enableSelectCriteria | enablePostSingulationMatch);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Tag18K6CInventory result : " + result);
        }

        private static void Set18K6CSingulationAlgorithmParameters(Linkage link, int radioHandle)
        {
            // Fixed.

            FixedQParms fqp = new FixedQParms();

            fqp.qValue = 1;
            fqp.retryCount = 1;
            fqp.repeatUntilNoTags = 1;
            if (1 == enableTagFocus)
            {
                fqp.toggleTarget = 0;
            }
            else
            {
                fqp.toggleTarget = 1;
            }

            var result = link.Set18K6CSingulationAlgorithmParameters(radioHandle, SingulationAlgorithm.FIXEDQ, fqp);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Set18K6CSingulationAlgorithmParameters Fixed Q result : " + result);

            // Dynamic.

            DynamicQParms dqp = new DynamicQParms();

            dqp.startQValue = 3;
            dqp.minQValue = 0;
            dqp.maxQValue = 7;
            dqp.retryCount = 1;
            dqp.thresholdMultiplier = 4;
            if (1 == enableTagSuppression)
            {
                dqp.toggleTarget = 0;
            }
            else
            {
                dqp.toggleTarget = 1;
            }

            result = link.Set18K6CSingulationAlgorithmParameters(radioHandle, SingulationAlgorithm.DYNAMICQ, dqp);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Set18K6CSingulationAlgorithmParameters Dynamic Q result : " + result);
        }

        private static void Set18K6CPostMatchCriteria(Linkage link, int radioHandle)
        {
            Result result;
            SingulationCriteria singulationCriteria = new SingulationCriteria();

            singulationCriteria.countCriteria = 1;

            singulationCriteria.pCriteria = new SingulationCriterion[singulationCriteria.countCriteria];

            singulationCriteria.pCriteria[0] = new SingulationCriterion();

            singulationCriteria.pCriteria[0].match = 1; // EPC data must match the mask to be returned
            // set to 0 if you EPC's that don't match

            // change the mask attributes as appropriate for the specific tag
            singulationCriteria.pCriteria[0].mask.offset = 80; // bit offset in the EPC of the 6th word
            singulationCriteria.pCriteria[0].mask.count = 16; // match 16 bits starting at offset
            singulationCriteria.pCriteria[0].mask.mask[0] = 0x00; // first byte of 6th word is 0
            singulationCriteria.pCriteria[0].mask.mask[1] = 0x24; // second byte of 6th word is 0x24

            result = link.Set18K6CPostMatchCriteria(radioHandle, singulationCriteria, 0);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Set18K6CPostMatchCriteria result : " + result);
        }

        private static void UseImpinjExtensions(Linkage link, int radioHandle)
        {
            ImpinjExtensions extensions = new ImpinjExtensions();

            // Retrieve current settings
            var result = link.RadioGetImpinjExtensions(radioHandle, extensions);
            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.RadioGetImpinjExtensions result : " + result);

            // now update based on the enables
            if (1 == enableTagFocus)
            {
                extensions.tagFocus = TagFocus.FOCUS_ENABLED;
            }
            else
            {
                extensions.tagFocus = TagFocus.FOCUS_DISABLED;
            }

            if (1 == enableFastId)
            {
                extensions.fastId = FastId.FAST_ID_ENABLED;
            }
            else
            {
                extensions.fastId = FastId.FAST_ID_DISABLED;
            }
            extensions.blockWriteMode = BlockWriteMode.AUTO;

            result = link.RadioSetImpinjExtensions(radioHandle, extensions);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.RadioSetImpinjExtensions result : " + result);
        }

        private static void Set18K6CQueryTagGroup(Linkage link, int radioHandle)
        {
            // Configure the query tag group to only get tags with the SL asserted
            TagGroup group = new TagGroup();
            group.target = SessionTarget.A;

            if (0 == enableSelectCriteria)
            {
                group.selected = Selected.SELECT_ALL;
            }
            else
            {
                group.selected = Selected.SELECT_ASSERTED;
            }

            if (1 == enableTagFocus)
            {
                group.session = Session.S1;
            }
            else
            {
                group.session = Session.S2;
            }

            var result = link.Set18K6CQueryTagGroup(radioHandle, group);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Set18K6CQueryTagGroup result : " + result);
        }

        private static void Get18K6CSelectCriteria(Linkage link, int radioHandle, SelectCriteria selectCriteria)
        {
            Result result = link.Get18K6CSelectCriteria(radioHandle, selectCriteria);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Get18K6CSelectCriteria result : " + result);
        }

        private static SelectCriteria Set18K6CSelectCriteria(Linkage link, int radioHandle)
        {
            SelectCriteria selectCriteria = new SelectCriteria();

            selectCriteria.countCriteria = 1;

            selectCriteria.pCriteria = new SelectCriterion[selectCriteria.countCriteria];

            selectCriteria.pCriteria[0] = new SelectCriterion();

            // change the mask attributes as appropriate for the specific tag
            selectCriteria.pCriteria[0].mask.bank = MemoryBank.EPC;
            selectCriteria.pCriteria[0].mask.offset = 112; // bit offset in the EPC memory bank, for the 6th word of the EPC
            selectCriteria.pCriteria[0].mask.count = 16; // 16 bits to match starting at offset
            selectCriteria.pCriteria[0].mask.mask[0] = 0x00; // first byte of 6th word is 0
            selectCriteria.pCriteria[0].mask.mask[1] = 0x24; // 2nd byte of 6th word is 24

            selectCriteria.pCriteria[0].action.target = Target.SELECTED;
            selectCriteria.pCriteria[0].action.action = rfid.Constants.Action.ASLINVA_DSLINVB;
            selectCriteria.pCriteria[0].action.enableTruncate = 0;


            var result = link.Set18K6CSelectCriteria(radioHandle, selectCriteria, 0);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.Set18K6CSelectCriteria result : " + result);

            return selectCriteria;
        }

        private static void AntennaPortSetConfiguration()
        {
            // TODO:  Add Set config for antenna port...
        }

        private static void AntennaPortGetConfiguration(Linkage link, int radioHandle)
        {
            AntennaPortConfig antennaPortConfig = new AntennaPortConfig();

            var result = link.AntennaPortGetConfiguration(radioHandle, 0, antennaPortConfig);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.AntennaPortGetConfig result : " + result);
            logger.Information("\tRadioHandle used              : " + radioHandle);
            logger.Information("\tLength found                  : " + antennaPortConfig.length);
            logger.Information("\tPowerLevel found              : " + antennaPortConfig.powerLevel);
            logger.Information("\tDwellTime found               : " + antennaPortConfig.dwellTime);
            logger.Information("\tNumberInventoryCycles found   : " + antennaPortConfig.numberInventoryCycles);
            logger.Information("\tphysicalRxPort found          : " + antennaPortConfig.physicalRxPort);
            logger.Information("\tPhysicalTxPort found          : " + antennaPortConfig.physicalTxPort);
            logger.Information("\tSense Threshold( glob ) found : " + antennaPortConfig.antennaSenseThreshold);
        }

        private static void SetAntennaPortStatus(Linkage link, int radioHandle, AntennaPortState portState)
        {
            Result result = link.AntennaPortSetState(radioHandle, 0, portState);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information($"link.AntennaPortSetStatus ( {portState} ) result : " + result);
        }

        private static void GetAntennaPortStatus(Linkage link, int radioHandle)
        {
            AntennaPortStatus antennaPortStatus = new AntennaPortStatus();

            var result = link.AntennaPortGetStatus(radioHandle, 0, antennaPortStatus);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.AntennaPortGetStatus result : " + result);
            logger.Information("\tRadioHandle used       : " + radioHandle);
            logger.Information("\tLength found           : " + antennaPortStatus.length);
            logger.Information("\tState found            : " + antennaPortStatus.state);
            logger.Information("\tSense Value found      : " + antennaPortStatus.antennaSenseValue);
        }

        private static void EnumerateLinkProfiles(Linkage link, int radioHandle)
        {
            UInt32 currentLinkProfile = 0;

            var result = link.RadioGetCurrentLinkProfile(radioHandle, ref currentLinkProfile);
            var savedLinkProfile = currentLinkProfile;

            for (currentLinkProfile = 0; currentLinkProfile < 4; ++currentLinkProfile)
            {
                result = link.RadioSetCurrentLinkProfile(radioHandle, currentLinkProfile);

                logger.Information(string.Empty);
                logger.Information(string.Empty);
                logger.Information("link.RadioSetCurrentLinkProfile ( " + currentLinkProfile + " ) result : " + result);
                logger.Information("\tRadioHandle used           : " + radioHandle);


                result = link.RadioGetCurrentLinkProfile(radioHandle, ref currentLinkProfile);

                logger.Information(string.Empty);
                logger.Information(string.Empty);
                logger.Information("link.RadioGetCurrentLinkProfile result : " + result);
                logger.Information("\tRadioHandle used           : " + radioHandle);
                logger.Information("\tCurrent Link Profile found : " + currentLinkProfile);
            }


            RadioLinkProfile linkProfile = new RadioLinkProfile();

            result = link.RadioGetLinkProfile(radioHandle, currentLinkProfile, linkProfile);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.RadioGetLinkProfile result : " + result);
            logger.Information("\tFound length           : " + linkProfile.length);
            logger.Information("\tFound profileId        : " + linkProfile.profileId);
            logger.Information("\tFound profileVersion   : " + linkProfile.profileVersion);
            logger.Information("\tFound profileProtocol  : " + linkProfile.profileProtocol);

            logger.Information("\tFound iso length              : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).length);
            logger.Information("\tFound iso modulationType      : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).modulationType);
            logger.Information("\tFound iso tari                : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).tari);
            logger.Information("\tFound iso data01Difference    : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).data01Difference);
            logger.Information("\tFound iso pulseWidth          : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).pulseWidth);
            logger.Information("\tFound iso rtCalibration       : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).rtCalibration);
            logger.Information("\tFound iso trCalibration       : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).trCalibration);
            logger.Information("\tFound iso divideRatio         : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).divideRatio);
            logger.Information("\tFound iso millerNumber        : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).millerNumber);
            logger.Information("\tFound iso trLinkFrequency     : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).trLinkFrequency);
            logger.Information("\tFound iso varT2Delay          : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).varT2Delay);
            logger.Information("\tFound iso rxDelay             : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).rxDelay);
            logger.Information("\tFound iso minT2Delay          : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).minT2Delay);
            logger.Information("\tFound iso txPropogationDelay  : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).length);

            // Restore link profile
            result = link.RadioSetCurrentLinkProfile(radioHandle, savedLinkProfile);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.RadioSetCurrentLinkProfile ( " + savedLinkProfile + " ) result : " + result);
            logger.Information("\tRadioHandle used           : " + radioHandle);
        }

        private static void SetRadioPowerState(Linkage link, int radioHandle, RadioPowerState powerState)
        {
            Result result = link.RadioSetPowerState(radioHandle, powerState);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information($"link.RadioSetPowerState ( {powerState} ) result : " + result);
        }

        private static void GetRadioPowerState(Linkage link, int radioHandle)
        {
            RadioPowerState powerState = new RadioPowerState();
            var result = link.RadioGetPowerState(radioHandle, ref powerState);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.RadioGetPowerState result : " + result);
            logger.Information("\tRadioHandle used    : " + radioHandle);
            logger.Information("\tPowerState found    : " + powerState);
        }

        private static void GetRadioOperationMode(Linkage link, int radioHandle)
        {
            RadioOperationMode operationMode = new RadioOperationMode();

            var result = link.RadioGetOperationMode(radioHandle, ref operationMode);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.RadioGetOperationMode result : " + result);
            logger.Information("\tRadioHandle used    : " + radioHandle);
            logger.Information("\tOperationMode found : " + operationMode);
        }

        private static void SetRadioOperationMode(Linkage link, int radioHandle, RadioOperationMode operationMode)
        {
            Result result = link.RadioSetOperationMode(radioHandle, operationMode);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information($"link.RadioSetOperationMode ( {operationMode} ) result : " + result);
        }

        private static void GetMacBootloaderVersion(Linkage link, int radioHandle)
        {
            MacBootLoaderVersion blVersion = new MacBootLoaderVersion();
            var result = link.MacGetBootLoaderVersion(radioHandle, blVersion);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacGetBootLoaderVersion result : " + result);
            logger.Information("\tmajor : " + blVersion.major);
            logger.Information("\tminor : " + blVersion.minor);
            logger.Information("\tmaintenance : " + blVersion.maintenance);
            logger.Information("\trelease : " + blVersion.release);
        }

        private static void GetMacVersion(Linkage link, int radioHandle)
        {
            MacVersion macVersion = new MacVersion();
            var result = link.MacGetVersion(radioHandle, macVersion);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.MacGetVersion result : " + result);
            logger.Information("\tmajor : " + macVersion.major);
            logger.Information("\tminor : " + macVersion.minor);
            logger.Information("\tmaintenance : " + macVersion.maintenance);
            logger.Information("\trelease : " + macVersion.release);
        }

        private static void RadioClose(Linkage link, int radioHandle)
        {
            Result result = link.RadioClose(radioHandle);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.RadioClose result : " + result);
        }

        private static int OpenRadio(Linkage link, RadioEnumeration re, int radioHandle)
        {
            Result result = link.RadioOpen(re.radioInfo[0].cookie, ref radioHandle, MacMode.DEFAULT);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.RadioOpen result : " + result);
            logger.Information("\tCookie used          : " + re.radioInfo[0].cookie);
            logger.Information("\tRadioHandle obtained : " + radioHandle);

            return radioHandle;
        }

        private static RadioEnumeration EnumerateRadios(Linkage link)
        {
            RadioEnumeration re = new RadioEnumeration();

            Result result = link.RetrieveAttachedRadiosList(re, 0);

            logger.Information(string.Empty);
            logger.Information(string.Empty);
            logger.Information("link.RetrieveAttachedRadiosList result : " + result);
            logger.Information("\tRadioEnum.length       : " + re.length);
            logger.Information("\tRadioEnum.totalLength  : " + re.totalLength);
            logger.Information("\tRadioEnum.countRadios  : " + re.countRadios);

            for (int index = 0; index < re.radioInfo.Length; ++index)
            {
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.length : " + re.radioInfo[index].length);

                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.driverVersion.major : " + re.radioInfo[index].driverVersion.major);
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.driverVersion.minor : " + re.radioInfo[index].driverVersion.minor);
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.driverVersion.maintenance : " + re.radioInfo[index].driverVersion.maintenance);
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.driverVersion.release : " + re.radioInfo[index].driverVersion.release);

                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.cookie   : " + re.radioInfo[index].cookie);
                logger.Information("\tRadio " + index + " RadioEnum.radioInfo.idLength : " + re.radioInfo[index].idLength);
                string uniqueId = "\tRadio " + index + " RadioEnum.radioInfo.uniqueId : ";

                int index2;

                for (index2 = 0; index2 < re.radioInfo[index].idLength; ++index2)
                {
                    uniqueId += (char) re.radioInfo[index].uniqueId[index2];
                }

                logger.Information(uniqueId);
            }

            return re;
        }

        private static void ShowLinkageLibraryVersion(Linkage link)
        {
            LibraryVersion version = new LibraryVersion();

            var result = link.Startup(version, 0);

            logger.Information(string.Empty);
            logger.Information(string.Empty);

            logger.Information("link.Startup result : " + result);
            logger.Information("\tVersion.major: " + version.major);
            logger.Information("\tVersion.minor: " + version.minor);
            logger.Information("\tVersion.maintenance: " + version.maintenance);
            logger.Information("\tVersion.release: " + version.release);
        }
    }
}