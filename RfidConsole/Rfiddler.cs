using System;
using System.Runtime.InteropServices;
using System.Threading;
using rfid;
using rfid.Constants;
using rfid.Structures;

namespace RfidConsole
{
    public class Rfiddler
    {
        static Int32 callbackCount = 0;
        static uint enableSelectCriteria = 0x00; // set to 0x01 to enable
        static uint enablePostSingulationMatch = 0x00; // set to 0x02 to enable
        static uint enableTagFocus = 0x00; // set to 0x01 to enable
        static uint enableTagSuppression = 0x00; // set to 0x01 to enable
        static uint enableFastId = 0x00; // set to 0x01 to enable

        public Int32 MyCallback
        (
            [In] Int32 handle,
            [In] UInt32 bufferLength,
            [In] IntPtr pBuffer,
            [In, Out] IntPtr context
        )
        {
            Byte[] packetBuffer = new Byte[bufferLength];
            Byte packetFlags;
            Int16 packetType;
            Int16 packetLength;

            Marshal.Copy(pBuffer, packetBuffer, 0, (Int32) bufferLength);

            packetFlags = packetBuffer[1];
            packetType = (Int16) ((packetBuffer[3] << 8) | packetBuffer[2]);
            packetLength = (Int16) ((packetBuffer[5] << 8) | packetBuffer[4]);
            string packetTypeString;
            packetTypeString = "Mac Packet rcv'd, PacketType = ";
            packetTypeString += string.Format("0x{0:X4}", packetType);

            Console.WriteLine(packetTypeString);

            // if its an  end packet, print the status string too.
            if (1 == packetType)
            {
                string packetStatusString;
                packetStatusString = "EndPacket Status = ";
                packetStatusString += string.Format("0x{0:X2}{1:X2}{2:X2}{3:X2}", packetBuffer[15], packetBuffer[14],
                    packetBuffer[13], packetBuffer[12]);

                Console.WriteLine(packetStatusString);
            }
            else if (5 == packetType)
            {
                Int16 length = (Int16) (((packetLength - 3) * 4) - (packetFlags >> 6));
                string packetEpcString;
                packetEpcString = "    EPC = ";
                for (int index = 0; index < length; ++index)
                {
                    packetEpcString += string.Format("{0:X2}", packetBuffer[20 + index]);
                }

                // Console.WriteLine(packetEpcString);
            }
            else if (6 == packetType) // access packet, print the flag word if non-zero, along with error indicators
            {
                Byte flagWord = packetBuffer[1];
                string packetFlagString;
                packetFlagString = "AccessPacket Flag = ";
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
                    Console.WriteLine(packetFlagString);
                }
            }

            ++callbackCount;

            if (40 == callbackCount)
            {
                return 1;
            }

            return 0;
        }

        // The following is just used to supply a pause between execution of the functions so users have a chance to 
        // view output details prior to scrolling off the screen...
        static void PAUSE(int seconds)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.Write("Pausing for " + seconds + " seconds ");

            for (int index = 0; index < seconds; ++index)
            {
                Thread.Sleep(1000);

                Console.Write(".");
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        public static void Start(string[] args)
        {
            Linkage link = new Linkage();

            ShowLinkageLibraryVersion(link);

            Rfiddler.PAUSE(1);

            RadioEnumeration re = EnumerateRadios(link);

            Int32 radioHandle = 0;

            radioHandle = OpenRadio(link, re, radioHandle);

            Rfiddler.PAUSE(1);

            RadioClose(link, radioHandle);

            Rfiddler.PAUSE(1);

            radioHandle = OpenRadio(link, re, radioHandle);

            Rfiddler.PAUSE(1);

            GetRadioOperationMode(link, radioHandle);

            Rfiddler.PAUSE(1);

            GetMacVersion(link, radioHandle);

            Rfiddler.PAUSE(1);

            GetMacBootloaderVersion(link, radioHandle);

            Rfiddler.PAUSE(1);

            SetRadioOperationMode(link, radioHandle, RadioOperationMode.NONCONTINUOUS);

            Rfiddler.PAUSE(1);

            GetRadioOperationMode(link, radioHandle);

            Rfiddler.PAUSE(1);

            GetRadioPowerState(link, radioHandle);

            Rfiddler.PAUSE(1);

            SetRadioPowerState(link, radioHandle, RadioPowerState.FULL);

            Rfiddler.PAUSE(1);

            GetRadioPowerState(link, radioHandle);

            Rfiddler.PAUSE(1);

            SetRadioPowerState(link, radioHandle, RadioPowerState.STANDBY);

            Rfiddler.PAUSE(1);

            GetRadioPowerState(link, radioHandle);

            Rfiddler.PAUSE(1);

            EnumerateLinkProfiles(link, radioHandle);

            Rfiddler.PAUSE(1);

            GetAntennaPortStatus(link, radioHandle);

            Rfiddler.PAUSE(1);

            SetAntennaPortStatus(link, radioHandle, AntennaPortState.DISABLED);

            Rfiddler.PAUSE(1);

            GetAntennaPortStatus(link, radioHandle);

            Rfiddler.PAUSE(1);

            SetAntennaPortStatus(link, radioHandle, AntennaPortState.ENABLED);

            Rfiddler.PAUSE(1);

            GetAntennaPortStatus(link, radioHandle);

            Rfiddler.PAUSE(1);

            AntennaPortGetConfiguration(link, radioHandle);

            AntennaPortSetConfiguration();
            
            Rfiddler.PAUSE(1);

            var selectCriteria = Set18K6CSelectCriteria(link, radioHandle);

            Rfiddler.PAUSE(1);

            Get18K6CSelectCriteria(link, radioHandle, selectCriteria);

            Rfiddler.PAUSE(1);

            Set18K6CQueryTagGroup(link, radioHandle);

            Rfiddler.PAUSE(1);

            UseImpinjExtensions(link, radioHandle);

            Rfiddler.PAUSE(1);

            Set18K6CPostMatchCriteria(link, radioHandle);

            Rfiddler.PAUSE(1);

            Set18K6CSingulationAlgorithmParameters(link, radioHandle);

            Rfiddler.PAUSE(1);

            //result = link.RadioSetResponseDataMode( radioHandle, ResponseType.DATA, ResponseMode.EXTENDED );

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine( "link.RadioSetResponseDataMode result : " + result );

            Tag18K6CInventory(link, radioHandle);

            Rfiddler.PAUSE(1);

            Tag18K6CRead(link, radioHandle);

            Rfiddler.PAUSE(1);

            Tag18K6CQT(link, radioHandle);

            Rfiddler.PAUSE(1);

            Tag18K6CBlockErase(link, radioHandle);

            Rfiddler.PAUSE(1);

            Tag18K6CKill(link, radioHandle);

            Tag18K6CLock(link, radioHandle);

            RadioTurnCarrierWaveOnRandom(link, radioHandle);

            Rfiddler.PAUSE(1);

            RegisterAccess(link, radioHandle);

            RadioClose(link, radioHandle);

            link.Shutdown();
        }

        private static void RegisterAccess(Linkage link, int radioHandle)
        {
            RegisterInfo info = new RegisterInfo();

            // test a banked register write            
            var result = link.MacWriteBankedRegister(radioHandle, 0x704, 2, 0xA5A5);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacWriteBankedRegister result : " + result);

            // should give an error, due to invalid selector
            result = link.MacWriteBankedRegister(radioHandle, 0x704, 16, 0xA5A5);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacWriteBankedRegister result (Expect Invalid Parameter) : " + result);

            Console.WriteLine();
            Console.WriteLine();


            // test a banked register read, last iteration i=16 should give an error
            UInt32 value = 0;
            for (UInt16 i = 0; i < 17; i++)
            {
                result = link.MacReadBankedRegister(radioHandle, 0x704, i, ref value);
                if (i < 16)
                {
                    Console.WriteLine("link.MacReadBankedRegister result : " + result);
                }
                else
                {
                    Console.WriteLine("link.MacReadBankedRegister result (Expect Invalid Parameter) : " + result);
                }
            }

            // Test OEMConfig Read/Write            
            UInt32[] readData = new UInt32[20];
            UInt32 numToRead = 16;
            result = link.MacReadOemData(radioHandle, 0x1f70, ref numToRead, readData);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacReadOemData result : " + result + ", numRead=" + numToRead);

            numToRead++;
            result = link.MacReadOemData(radioHandle, 0x1f70, ref numToRead, readData);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacReadOemData result (Expect Invalid Parameter) : " + result + ", numRead=" + numToRead);

            UInt32 numToWrite = 16;
            result = link.MacWriteOemData(radioHandle, 0x1f70, ref numToWrite, readData);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacWriteOemData result : " + result + ", numWritten=" + numToWrite);

            numToWrite++;
            result = link.MacWriteOemData(radioHandle, 0x1f70, ref numToWrite, readData);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacWriteOemData result (Expect Invalid Parameter) : " + result + ", numWritten=" + numToWrite);


            // force a mac error writing an invalid address and confirm mac error and macLastError
            // can be retrieved and reflect the same value
            result = link.MacWriteRegister(radioHandle, 0xFFFF, 0);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacWriteRegister result (Expect Invalid Parameter) : " + result);
            UInt32 macError = 0;
            UInt32 macLastError = 0;
            result = link.MacGetError(radioHandle, ref macError, ref macLastError);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacGetError (" + macError + ", " + macLastError + ") result : " + result);

            // confirm a successful register access clears the mac error but not the lastMacError
            UInt32 macVer = 0;
            result = link.MacReadRegister(radioHandle, 0x0000, ref macVer);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacReadRegister result : " + result);
            result = link.MacGetError(radioHandle, ref macError, ref macLastError);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacGetError (" + macError + ", " + macLastError + ") result : " + result);
        }

        private static void RadioTurnCarrierWaveOnRandom(Linkage link, int radioHandle)
        {
            Result result;
            var program = new Rfiddler();

            RandomCwParms randomCwParms = new RandomCwParms();

            // Enable Tx Random Data status packet by setting bits 7 and 0 in HST_CMNDIAGS (0x201)           
            UInt32 hstCmnDiags = 0;
            result = link.MacReadRegister(radioHandle, 0x0201, ref hstCmnDiags);
            Console.WriteLine();
            Console.WriteLine("link.MacReadRegister result : " + result);
            hstCmnDiags |= 0x81;
            result = link.MacWriteRegister(radioHandle, 0x0201, hstCmnDiags);
            Console.WriteLine();
            Console.WriteLine("link.MacWriteRegister result : " + result);
            Console.WriteLine();

            Console.WriteLine("Starting random cw test:");
            Console.WriteLine("Limiting to 40 MAC packets");

#if TEST_NULL_RANDOM_CW_CALLBACK
            randomCwParms.callback = null;
            randomCwParms.context = (IntPtr) null;
            randomCwParms.callbackCode = (IntPtr) null;
#else
            randomCwParms.callback = new CallbackDelegate(program.MyCallback);
            randomCwParms.context = IntPtr.Zero;
            randomCwParms.callbackCode = IntPtr.Zero;
#endif
            randomCwParms.duration = 10000;

            result = link.RadioTurnCarrierWaveOnRandom(radioHandle, randomCwParms);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RadioTurnCarrierWaveOnRandom result : " + result);
        }

        private static void Tag18K6CLock(Linkage link, int radioHandle)
        {

            var program = new Rfiddler();

            LockParms lockParms = new LockParms();

            lockParms.common.tagStopCount = 0;
            lockParms.common.callback = new CallbackDelegate(program.MyCallback);
            lockParms.common.context = IntPtr.Zero;
            lockParms.common.callbackCode = IntPtr.Zero;

            lockParms.lockCmdParms.permissions.killPasswordPermissions = PasswordPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.accessPasswordPermissions = PasswordPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.epcMemoryBankPermissions = MemoryPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.tidMemoryBankPermissions = MemoryPermission.NO_CHANGE;
            lockParms.lockCmdParms.permissions.userMemoryBankPermissions = MemoryPermission.NO_CHANGE;

            lockParms.accessPassword = 0x0;

            //Removed LOCK test - do not want to accidentally have tags locked unexpectedly...
            // var result = link.Tag18K6CLock(radioHandle, lockParms, 0);

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine("link.Tag18K6CLock result : " + result);
        }

        private static void Tag18K6CKill(Linkage link, int radioHandle)
        {
            var program = new Rfiddler();

            KillParms killParms = new KillParms();

            killParms.common.tagStopCount = 0;
            killParms.common.callback = new CallbackDelegate(program.MyCallback);
            killParms.common.context = IntPtr.Zero;
            killParms.common.callbackCode = IntPtr.Zero;

            killParms.accessPassword = 0x0;
            killParms.killCmdParms.killPassword = 0x0;

            // Removed KILL test - do not want to accidentally have tags killed unexpectedly...
            //var result = link.Tag18K6CKill(radioHandle, killParms, 0);

            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine("link.Tag18K6CKill result : " + result);
        }

        private static void Tag18K6CBlockErase(Linkage link, int radioHandle)
        {
            var program = new Rfiddler();

            BlockEraseParms blockEraseParms = new BlockEraseParms();

            Console.WriteLine("Starting basic block erase test:");
            Console.WriteLine("Limiting of 40 MAC packets");

            blockEraseParms.common.tagStopCount = 0;
            blockEraseParms.common.callback = new CallbackDelegate(program.MyCallback);
            blockEraseParms.common.context = IntPtr.Zero;
            blockEraseParms.common.callbackCode = IntPtr.Zero;

            blockEraseParms.blockEraseCmdParms.bank = MemoryBank.RESERVED;
            blockEraseParms.blockEraseCmdParms.offset = 0;
            blockEraseParms.blockEraseCmdParms.count = 4;

            blockEraseParms.accessPassword = 0x0;

            var result = link.Tag18K6CBlockErase(radioHandle, blockEraseParms, 0);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.Tag18K6CBlockErase result : " + result);
        }

        private static void Tag18K6CQT(Linkage link, int radioHandle)
        {
            var program = new Rfiddler();

            QTParms qtParms = new QTParms();


            Console.WriteLine("Starting basic QT test:");
            Console.WriteLine("Limiting of 40 MAC packets");

            qtParms.common.tagStopCount = 0;
            qtParms.common.callback = new CallbackDelegate(program.MyCallback);
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

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.Tag18K6CQT result : " + result);
        }

        private static void Tag18K6CRead(Linkage link, int radioHandle)
        {
            Rfiddler program = new Rfiddler();

            ReadParms readParms = new ReadParms();

            Console.WriteLine("Starting basic read test:");
            Console.WriteLine("Limiting of 40 MAC packets");

            readParms.common.tagStopCount = 0;
            readParms.common.callback = new CallbackDelegate(program.MyCallback);
            readParms.common.context = IntPtr.Zero;
            readParms.common.callbackCode = IntPtr.Zero;

            readParms.readCmdParms.bank = MemoryBank.EPC;
            readParms.readCmdParms.offset = 2;
            readParms.readCmdParms.count = 6;

            readParms.accessPassword = 0x0;

            var result = link.Tag18K6CRead(radioHandle, readParms, 0);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.Tag18K6CRead result : " + result);
        }

        private static void Tag18K6CInventory(Linkage link, int radioHandle)
        {
            Console.WriteLine("Starting basic inventory test:");
            Console.WriteLine("Limiting of 40 MAC packets");


            Rfiddler program = new Rfiddler();

            InventoryParms inventoryParms = new InventoryParms();

            inventoryParms.common = new CommonParms();

            inventoryParms.common.tagStopCount = 0;
            inventoryParms.common.callback = new CallbackDelegate(program.MyCallback);
            inventoryParms.common.context = IntPtr.Zero;
            inventoryParms.common.callbackCode = IntPtr.Zero;

            var result = link.Tag18K6CInventory(radioHandle, inventoryParms, enableSelectCriteria | enablePostSingulationMatch);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.Tag18K6CInventory result : " + result);
        }

        private static void Set18K6CSingulationAlgorithmParameters(Linkage link, int radioHandle)
        {
            Result result;
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

            result = link.Set18K6CSingulationAlgorithmParameters(radioHandle, SingulationAlgorithm.FIXEDQ, fqp);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.Set18K6CSingulationAlgorithmParameters Fixed Q result : " + result);


            Rfiddler.PAUSE(1);


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

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.Set18K6CSingulationAlgorithmParameters Dynamic Q result : " + result);
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

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.Set18K6CPostMatchCriteria result : " + result);
        }

        private static void UseImpinjExtensions(Linkage link, int radioHandle)
        {
            ImpinjExtensions extensions = new ImpinjExtensions();

            // retrieve current settings
            var result = link.RadioGetImpinjExtensions(radioHandle, extensions);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RadioGetImpinjExtensions result : " + result);


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

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RadioSetImpinjExtensions result : " + result);
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

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.Set18K6CQueryTagGroup result : " + result);
        }

        private static void Get18K6CSelectCriteria(Linkage link, int radioHandle, SelectCriteria selectCriteria)
        {
            Result result = link.Get18K6CSelectCriteria(radioHandle, selectCriteria);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.Get18K6CSelectCriteria result : " + result);
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

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.Set18K6CSelectCriteria result : " + result);

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

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.AntennaPortGetConfig result : " + result);
            Console.WriteLine("\tRadioHandle used              : " + radioHandle);
            Console.WriteLine("\tLength found                  : " + antennaPortConfig.length);
            Console.WriteLine("\tPowerLevel found              : " + antennaPortConfig.powerLevel);
            Console.WriteLine("\tDwellTime found               : " + antennaPortConfig.dwellTime);
            Console.WriteLine("\tNumberInventoryCycles found   : " + antennaPortConfig.numberInventoryCycles);
            Console.WriteLine("\tphysicalRxPort found          : " + antennaPortConfig.physicalRxPort);
            Console.WriteLine("\tPhysicalTxPort found          : " + antennaPortConfig.physicalTxPort);
            Console.WriteLine("\tSense Threshold( glob ) found : " + antennaPortConfig.antennaSenseThreshold);
        }

        private static void SetAntennaPortStatus(Linkage link, int radioHandle, AntennaPortState portState)
        {
            Result result = link.AntennaPortSetState(radioHandle, 0, portState);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.AntennaPortSetStatus ( disabled ) result : " + result);
        }

        private static void GetAntennaPortStatus(Linkage link, int radioHandle)
        {
            AntennaPortStatus antennaPortStatus = new AntennaPortStatus();

            var result = link.AntennaPortGetStatus(radioHandle, 0, antennaPortStatus);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.AntennaPortGetStatus result : " + result);
            Console.WriteLine("\tRadioHandle used       : " + radioHandle);
            Console.WriteLine("\tLength found           : " + antennaPortStatus.length);
            Console.WriteLine("\tState found            : " + antennaPortStatus.state);
            Console.WriteLine("\tSense Value found      : " + antennaPortStatus.antennaSenseValue);
        }

        private static void EnumerateLinkProfiles(Linkage link, int radioHandle)
        {
            Result result;
            UInt32 currentLinkProfile = 0;

            result = link.RadioGetCurrentLinkProfile(radioHandle, ref currentLinkProfile);
            var savedLinkProfile = currentLinkProfile;

            for (currentLinkProfile = 0; currentLinkProfile < 4; ++currentLinkProfile)
            {
                result = link.RadioSetCurrentLinkProfile(radioHandle, currentLinkProfile);

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("link.RadioSetCurrentLinkProfile ( " + currentLinkProfile + " ) result : " + result);
                Console.WriteLine("\tRadioHandle used           : " + radioHandle);


                result = link.RadioGetCurrentLinkProfile(radioHandle, ref currentLinkProfile);

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("link.RadioGetCurrentLinkProfile result : " + result);
                Console.WriteLine("\tRadioHandle used           : " + radioHandle);
                Console.WriteLine("\tCurrent Link Profile found : " + currentLinkProfile);

                Rfiddler.PAUSE(1);
            }


            RadioLinkProfile linkProfile = new RadioLinkProfile();

            result = link.RadioGetLinkProfile(radioHandle, currentLinkProfile, linkProfile);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RadioGetLinkProfile result : " + result);
            Console.WriteLine("\tFound length           : " + linkProfile.length);
            Console.WriteLine("\tFound profileId        : " + linkProfile.profileId);
            Console.WriteLine("\tFound profileVersion   : " + linkProfile.profileVersion);
            Console.WriteLine("\tFound profileProtocol  : " + linkProfile.profileProtocol);

            Console.WriteLine("\tFound iso length              : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).length);
            Console.WriteLine("\tFound iso modulationType      : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).modulationType);
            Console.WriteLine("\tFound iso tari                : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).tari);
            Console.WriteLine("\tFound iso data01Difference    : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).data01Difference);
            Console.WriteLine("\tFound iso pulseWidth          : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).pulseWidth);
            Console.WriteLine("\tFound iso rtCalibration       : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).rtCalibration);
            Console.WriteLine("\tFound iso trCalibration       : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).trCalibration);
            Console.WriteLine("\tFound iso divideRatio         : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).divideRatio);
            Console.WriteLine("\tFound iso millerNumber        : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).millerNumber);
            Console.WriteLine("\tFound iso trLinkFrequency     : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).trLinkFrequency);
            Console.WriteLine("\tFound iso varT2Delay          : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).varT2Delay);
            Console.WriteLine("\tFound iso rxDelay             : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).rxDelay);
            Console.WriteLine("\tFound iso minT2Delay          : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).minT2Delay);
            Console.WriteLine("\tFound iso txPropogationDelay  : " + ((RadioLinkProfileConfig_ISO18K6C) linkProfile.profileConfig).length);

            // Restore link profile
            result = link.RadioSetCurrentLinkProfile(radioHandle, savedLinkProfile);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RadioSetCurrentLinkProfile ( " + savedLinkProfile + " ) result : " + result);
            Console.WriteLine("\tRadioHandle used           : " + radioHandle);
        }

        private static void SetRadioPowerState(Linkage link, int radioHandle, RadioPowerState powerState)
        {
            Result result = link.RadioSetPowerState(radioHandle, powerState);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RadioSetPowerState ( full ) result : " + result);
        }

        private static void GetRadioPowerState(Linkage link, int radioHandle)
        {
            RadioPowerState powerState = new RadioPowerState();
            var result = link.RadioGetPowerState(radioHandle, ref powerState);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RadioGetPowerState result : " + result);
            Console.WriteLine("\tRadioHandle used    : " + radioHandle);
            Console.WriteLine("\tPowerState found    : " + powerState);
        }

        private static void SetRadioOperationMode(Linkage link, int radioHandle, RadioOperationMode operationMode)
        {
            Result result = link.RadioSetOperationMode(radioHandle, operationMode);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RadioSetOperationMode ( continuous ) result : " + result);
        }

        private static void GetMacBootloaderVersion(Linkage link, int radioHandle)
        {
            MacBootLoaderVersion blVersion = new MacBootLoaderVersion();
            var result = link.MacGetBootLoaderVersion(radioHandle, blVersion);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacGetBootLoaderVersion result : " + result);
            Console.WriteLine("\tmajor : " + blVersion.major);
            Console.WriteLine("\tminor : " + blVersion.minor);
            Console.WriteLine("\tmaintenance : " + blVersion.maintenance);
            Console.WriteLine("\trelease : " + blVersion.release);
        }

        private static void GetMacVersion(Linkage link, int radioHandle)
        {
            MacVersion macVersion = new MacVersion();
            var result = link.MacGetVersion(radioHandle, macVersion);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.MacGetVersion result : " + result);
            Console.WriteLine("\tmajor : " + macVersion.major);
            Console.WriteLine("\tminor : " + macVersion.minor);
            Console.WriteLine("\tmaintenance : " + macVersion.maintenance);
            Console.WriteLine("\trelease : " + macVersion.release);
        }

        private static void GetRadioOperationMode(Linkage link, int radioHandle)
        {
            RadioOperationMode operationMode = new RadioOperationMode();

            var result = link.RadioGetOperationMode(radioHandle, ref operationMode);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RadioGetOperationMode result : " + result);
            Console.WriteLine("\tRadioHandle used    : " + radioHandle);
            Console.WriteLine("\tOperationMode found : " + operationMode);
        }

        private static void RadioClose(Linkage link, int radioHandle)
        {
            Result result = link.RadioClose(radioHandle);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RadioClose result : " + result);
        }

        private static int OpenRadio(Linkage link, RadioEnumeration re, int radioHandle)
        {
            Result result = link.RadioOpen(re.radioInfo[0].cookie, ref radioHandle, MacMode.DEFAULT);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RadioOpen result : " + result);
            Console.WriteLine("\tCookie used          : " + re.radioInfo[0].cookie);
            Console.WriteLine("\tRadioHandle obtained : " + radioHandle);

            return radioHandle;
        }

        private static RadioEnumeration EnumerateRadios(Linkage link)
        {
            RadioEnumeration re = new RadioEnumeration();

            Result result = link.RetrieveAttachedRadiosList(re, 0);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("link.RetrieveAttachedRadiosList result : " + result);
            Console.WriteLine("\tRadioEnum.length       : " + re.length);
            Console.WriteLine("\tRadioEnum.totalLength  : " + re.totalLength);
            Console.WriteLine("\tRadioEnum.countRadios  : " + re.countRadios);

            Rfiddler.PAUSE(5);

            for (int index = 0; index < re.radioInfo.Length; ++index)
            {
                Console.WriteLine("\tRadio " + index + " RadioEnum.radioInfo.length : " + re.radioInfo[index].length);

                Console.WriteLine("\tRadio " + index + " RadioEnum.radioInfo.driverVersion.major : " + re.radioInfo[index].driverVersion.major);
                Console.WriteLine("\tRadio " + index + " RadioEnum.radioInfo.driverVersion.minor : " + re.radioInfo[index].driverVersion.minor);
                Console.WriteLine("\tRadio " + index + " RadioEnum.radioInfo.driverVersion.maintenance : " + re.radioInfo[index].driverVersion.maintenance);
                Console.WriteLine("\tRadio " + index + " RadioEnum.radioInfo.driverVersion.release : " + re.radioInfo[index].driverVersion.release);

                Console.WriteLine("\tRadio " + index + " RadioEnum.radioInfo.cookie   : " + re.radioInfo[index].cookie);
                Console.WriteLine("\tRadio " + index + " RadioEnum.radioInfo.idLength : " + re.radioInfo[index].idLength);
                Console.Write("\tRadio " + index + " RadioEnum.radioInfo.uniqueId : ");

                int index2;

                for (index2 = 0; index2 < re.radioInfo[index].idLength; ++index2)
                {
                    Console.Write((char) re.radioInfo[index].uniqueId[index2]);
                }

                Console.WriteLine();

                Rfiddler.PAUSE(1);
            }

            return re;
        }

        private static void ShowLinkageLibraryVersion(Linkage link)
        {
            LibraryVersion version = new LibraryVersion();

            var result = link.Startup(version, 0);

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("link.Startup result : " + result);
            Console.WriteLine("\tVersion.major: " + version.major);
            Console.WriteLine("\tVersion.minor: " + version.minor);
            Console.WriteLine("\tVersion.maintenance: " + version.maintenance);
            Console.WriteLine("\tVersion.release: " + version.release);
        }
    }
}