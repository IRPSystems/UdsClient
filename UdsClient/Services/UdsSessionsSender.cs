
using Peak.Can.IsoTp;
using Peak.Can.Uds;
using System.Runtime.InteropServices;

namespace UdsClient.Services
{
    public class UdsSessionsSender
    {
		private cantp_handle _client_handle;
		private uds_msgconfig _config;

		public void Init()
		{
			uds_status status;
			uint dw_buffer;
			_config = new uds_msgconfig();

			// Set the PCAN-Channel to use
			_client_handle = cantp_handle.PCANTP_HANDLE_USBBUS1; // TODO: modify the value according to your available PCAN devices.

			// Initializing of the UDS Communication session
			status = UDSApi.Initialize_2013(_client_handle, cantp_baudrate.PCANTP_BAUDRATE_500K);
			
			// Define TimeOuts
			dw_buffer = CanTpApi.PCANTP_ISO_TIMEOUTS_15765_4;
			status = UDSApi.SetValue_2013(_client_handle, uds_parameter.PUDS_PARAMETER_ISO_TIMEOUTS, ref dw_buffer, sizeof(uint));
			
			// Define Network Address Information used for all the tests
			_config.can_id = (uint)0xFFFFFFFF;
			_config.can_msgtype = cantp_can_msgtype.PCANTP_CAN_MSGTYPE_STANDARD;
			_config.nai.protocol = uds_msgprotocol.PUDS_MSGPROTOCOL_ISO_15765_2_11B_NORMAL;
			_config.nai.target_type = cantp_isotp_addressing.PCANTP_ISOTP_ADDRESSING_PHYSICAL;
			_config.type = uds_msgtype.PUDS_MSGTYPE_USDT;
			_config.nai.source_addr = (ushort)uds_address.PUDS_ADDRESS_ISO_15765_4_ADDR_TEST_EQUIPMENT;
			_config.nai.target_addr = (ushort)uds_address.PUDS_ADDRESS_ISO_15765_4_ADDR_ECU_1;
		}

		static uint Reverse32(uint v)
		{

			return (v & 0x000000FF) << 24 | (v & 0x0000FF00) << 8 | (v & 0x00FF0000) >> 8 | ((v & 0xFF000000) >> 24) & 0x000000FF;
		}

		public void UInt32ToBytes(uint dw_buffer, byte[] byte_buffer)
		{
			byte_buffer[0] = (byte)(dw_buffer & 0x000000FF);
			byte_buffer[1] = (byte)((dw_buffer & 0x0000FF00) >> 8);
			byte_buffer[2] = (byte)((dw_buffer & 0x00FF0000) >> 16);
			byte_buffer[3] = (byte)((dw_buffer & 0xFF000000) >> 24);
		}

		public void SendDiagnosticSessionControl(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_sessioninfo session_info = new uds_sessioninfo();
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();

			
			// Read default session information Server is not yet known (status will be PUDS_ERROR_NOT_INITIALIZED)
			// yet the API will still set session info to default values
			session_info.nai = config.nai;
			int session_size = Marshal.SizeOf(session_info);
			IntPtr session_ptr = Marshal.AllocHGlobal(session_size);
			Marshal.StructureToPtr(session_info, session_ptr, true);
			status = UDSApi.GetValue_2013(channel, uds_parameter.PUDS_PARAMETER_SESSION_INFO, session_ptr, (uint)session_size);
			session_info = (uds_sessioninfo)Marshal.PtrToStructure(session_ptr, typeof(uds_sessioninfo));
			Marshal.FreeHGlobal(session_ptr);
			
			// Set Diagnostic session to DEFAULT (to get session information)
			status = UDSApi.SvcDiagnosticSessionControl_2013(channel, config, out request, UDSApi.uds_svc_param_dsc.PUDS_SVC_PARAM_DSC_DS);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			//if (UDSApi.StatusIsOk_2013(status))
			//	display_uds_msg(ref confirmation, ref response, false);
			//else
			//	display_uds_msg_request(ref request, false);



			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
			// Read current session information
			session_info = new uds_sessioninfo();
			session_info.nai = config.nai;
			session_ptr = Marshal.AllocHGlobal(session_size);
			Marshal.StructureToPtr(session_info, session_ptr, true);
			status = UDSApi.GetValue_2013(channel, uds_parameter.PUDS_PARAMETER_SESSION_INFO, session_ptr, (uint)session_size);
			session_info = (uds_sessioninfo)Marshal.PtrToStructure(session_ptr, typeof(uds_sessioninfo));
			Marshal.FreeHGlobal(session_ptr);
			

			// Set Diagnostic session to PROGRAMMING
			status = UDSApi.SvcDiagnosticSessionControl_2013(channel, config, out request, UDSApi.uds_svc_param_dsc.PUDS_SVC_PARAM_DSC_ECUPS);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			Console.WriteLine(" UDS_SvcDiagnosticSessionControl_2013: {0}", (int)status);
			
			
			//if (UDSApi.StatusIsOk_2013(status))
			//	display_uds_msg(ref confirmation, ref response, false);
			//else
			//	display_uds_msg_request(ref request, false);


			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
			// Read current session information
			session_info = new uds_sessioninfo();
			session_info.nai = config.nai;
			session_ptr = Marshal.AllocHGlobal(session_size);
			Marshal.StructureToPtr(session_info, session_ptr, true);
			status = UDSApi.GetValue_2013(channel, uds_parameter.PUDS_PARAMETER_SESSION_INFO, session_ptr, (uint)session_size);
			session_info = (uds_sessioninfo)Marshal.PtrToStructure(session_ptr, typeof(uds_sessioninfo));
			Marshal.FreeHGlobal(session_ptr);
			Thread.Sleep(2000);
			
			// Set Diagnostic session back to DEFAULT
			status = UDSApi.SvcDiagnosticSessionControl_2013(channel, config, out request, UDSApi.uds_svc_param_dsc.PUDS_SVC_PARAM_DSC_DS);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			
			//if (UDSApi.StatusIsOk_2013(status))
			//	display_uds_msg(ref confirmation, ref response, false);
			//else
			//	display_uds_msg_request(ref request, false);


			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			Thread.Sleep(2000);
			
		}

		public void testECUReset(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();

			
			// Sends a physical ECUReset message
			status = UDSApi.SvcECUReset_2013(channel, config, out request, UDSApi.uds_svc_param_er.PUDS_SVC_PARAM_ER_SR);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			//if (UDSApi.StatusIsOk_2013(status))
			//	display_uds_msg(ref confirmation, ref response, false);
			//else
			//	display_uds_msg_request(ref request, false);

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service SecurityAccess</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testSecurityAccess(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			uint dw_buffer;
			byte[] security_access_data = new byte[4];
			uint value_little_endian;

			
			// Sends a physical SecurityAccess message
			value_little_endian = 0xF0A1B2C3;
			dw_buffer = Reverse32(value_little_endian); // use helper function to set MSB as 1st byte in the buffer (Win32 uses little endian format)
			UInt32ToBytes(dw_buffer, security_access_data);
			status = UDSApi.SvcSecurityAccess_2013(channel, config, out request, UDSApi.PUDS_SVC_PARAM_SA_RSD_1, security_access_data, 4);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			//if (UDSApi.StatusIsOk_2013(status))
			//	display_uds_msg(ref confirmation, ref response, false);
			//else
			//	display_uds_msg_request(ref request, false);

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service CommunicationControl</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testCommunicationControl(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();

			
			// Sends a physical CommunicationControl message
			status = UDSApi.SvcCommunicationControl_2013(channel, config, out request,
				UDSApi.uds_svc_param_cc.PUDS_SVC_PARAM_CC_ERXTX, UDSApi.PUDS_SVC_PARAM_CC_FLAG_APPL | UDSApi.PUDS_SVC_PARAM_CC_FLAG_NWM | UDSApi.PUDS_SVC_PARAM_CC_FLAG_DENWRIRO);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			//if (UDSApi.StatusIsOk_2013(status))
			//	display_uds_msg(ref confirmation, ref response, false);
			//else
			//	display_uds_msg_request(ref request, false);

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>UDS Call Service TesterPresent</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testTesterPresent(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg[] tresponse = new uds_msg[1];
			uds_msg response = new uds_msg();
			uint response_count = 0;
			uds_msg confirmation = new uds_msg();

			
			// Sends a physical TesterPresent message
			status = UDSApi.SvcTesterPresent_2013(channel, config, out request, UDSApi.uds_svc_param_tp.PUDS_SVC_PARAM_TP_ZSUBF);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			//if (UDSApi.StatusIsOk_2013(status))
			//	display_uds_msg(ref confirmation, ref response, false);
			//else
			//	display_uds_msg_request(ref request, false);

			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical TesterPresent message with no positive response
			config.type = uds_msgtype.PUDS_MSGTYPE_FLAG_NO_POSITIVE_RESPONSE;
			status = UDSApi.SvcTesterPresent_2013(channel, config, out request, UDSApi.uds_svc_param_tp.PUDS_SVC_PARAM_TP_ZSUBF);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			//if (UDSApi.StatusIsOk_2013(status))
			//	display_uds_msg(ref confirmation, ref response, false);
			//else
			//	display_uds_msg_request(ref request, true);

			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
			// Sends a functional TesterPresent message
			config.type = uds_msgtype.PUDS_MSGTYPE_USDT;
			config.nai.target_type = cantp_isotp_addressing.PCANTP_ISOTP_ADDRESSING_FUNCTIONAL;
			config.nai.target_addr = (ushort)uds_address.PUDS_ADDRESS_ISO_15765_4_ADDR_OBD_FUNCTIONAL;
			status = UDSApi.SvcTesterPresent_2013(channel, config, out request, UDSApi.uds_svc_param_tp.PUDS_SVC_PARAM_TP_ZSUBF);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForServiceFunctional_2013(channel, ref request, 1, true, tresponse, out response_count, out confirmation);
			
			//if (UDSApi.StatusIsOk_2013(status))
			//	display_uds_msg(ref confirmation, ref tresponse[0], false);
			//else
			//	display_uds_msg_request(ref request, false);

			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref tresponse[0]);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a functional TesterPresent message with no positive response
			config.type = uds_msgtype.PUDS_MSGTYPE_FLAG_NO_POSITIVE_RESPONSE;
			config.nai.target_type = cantp_isotp_addressing.PCANTP_ISOTP_ADDRESSING_FUNCTIONAL;
			config.nai.target_addr = (ushort)uds_address.PUDS_ADDRESS_ISO_15765_4_ADDR_OBD_FUNCTIONAL;
			status = UDSApi.SvcTesterPresent_2013(channel, config, out request, UDSApi.uds_svc_param_tp.PUDS_SVC_PARAM_TP_ZSUBF);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForServiceFunctional_2013(channel, ref request, 1, true, tresponse, out response_count, out confirmation);
			
			//if (UDSApi.StatusIsOk_2013(status) && response_count != 0)
			//	display_uds_msg(ref confirmation, ref tresponse[0], false);
			//else
			//	display_uds_msg_request(ref request, true);

			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref tresponse[0]);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service SecuredDataTransmission</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testSecuredDataTransmission(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg ecureset_request = new uds_msg();
			uds_msg confirmation = new uds_msg();
			uint dw_buffer;
			byte[] security_data_request_record = new byte[4];
			uint value_little_endian;
			ushort administrative_parameter;
			byte signature_encryption_calculation;
			ushort anti_replay_counter;
			byte internal_service_identifier;
			byte[] service_specific_parameters = new byte[4];
			uint service_specific_parameters_size;
			byte[] signature_mac = new byte[6];
			ushort signature_size;

			
			// Sends a physical SecuredDataTransmission/2013 message
			value_little_endian = 0xF0A1B2C3;
			dw_buffer = Reverse32(value_little_endian); // use helper function to set MSB as 1st byte in the buffer (Win32 uses little endian format)
			UInt32ToBytes(dw_buffer, security_data_request_record);
			status = UDSApi.SvcSecuredDataTransmission_2013(channel, config, out request, security_data_request_record, 4);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			//if (UDSApi.StatusIsOk_2013(status))
			//	display_uds_msg(ref confirmation, ref response, false);
			//else
			//	display_uds_msg_request(ref request, false);

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical SecuredDataTransmission/2013 message prepared with PUDS_ONLY_PREPARE_REQUEST option
			status = UDSApi.SvcECUReset_2013(UDSApi.PUDS_ONLY_PREPARE_REQUEST, config, out ecureset_request, UDSApi.uds_svc_param_er.PUDS_SVC_PARAM_ER_HR);
			if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
			{
				byte[] ecureset_data = new byte[ecureset_request.msg.Msgdata_any_Copy.length];
				for (int i = 0; i < ecureset_request.msg.Msgdata_any_Copy.length; i++)
				{
					byte val = 0;
					CanTpApi.getData_2016(ref ecureset_request.msg, i, out val);
					ecureset_data[i] = val;
				}
				status = UDSApi.SvcSecuredDataTransmission_2013(channel, config, out request, ecureset_data, ecureset_request.msg.Msgdata_any_Copy.length);
			}
			if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			//if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
			//	display_uds_msg(ref confirmation, ref response, false);
			//else
			//	display_uds_msg_request(ref request, false);

			// Free messages
			status = UDSApi.MsgFree_2013(ref ecureset_request);
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
			// Sends a physical SecuredDataTransmission/2020 message
			administrative_parameter = UDSApi.PUDS_SVC_PARAM_APAR_REQUEST_MSG_FLAG | UDSApi.PUDS_SVC_PARAM_APAR_REQUEST_RESPONSE_SIGNATURE_FLAG | UDSApi.PUDS_SVC_PARAM_APAR_SIGNED_MSG_FLAG;
			signature_encryption_calculation = 0x0;
			anti_replay_counter = 0x0124;
			internal_service_identifier = 0x2E;
			service_specific_parameters[0] = 0xF1;
			service_specific_parameters[1] = 0x23;
			service_specific_parameters[2] = 0xAA;
			service_specific_parameters[3] = 0x55;
			service_specific_parameters_size = 4;
			signature_mac[0] = 0xDB;
			signature_mac[1] = 0xD1;
			signature_mac[2] = 0x0E;
			signature_mac[3] = 0xDC;
			signature_mac[4] = 0x55;
			signature_mac[5] = 0xAA;
			signature_size = 0x0006;
			status = UDSApi.SvcSecuredDataTransmission_2020(channel, config, out request, administrative_parameter, signature_encryption_calculation, anti_replay_counter, internal_service_identifier, service_specific_parameters, service_specific_parameters_size, signature_mac, signature_size);
			if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			//if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
			//	display_uds_msg(ref confirmation, ref response, false);
			//else
			//	display_uds_msg_request(ref request, false);

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service ControlDTCSetting</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testControlDTCSetting(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			uint dw_buffer;
			byte[] dtc_setting_control_option_record = new byte[4];
			uint value_little_endian;

			
			// Sends a physical ControlDTCSetting message
			Console.WriteLine(); Console.WriteLine(); Console.WriteLine("Sends a physical ControlDTCSetting message: ");
			value_little_endian = 0xF1A1B2EE;
			dw_buffer = Reverse32(value_little_endian); // use helper function to set MSB as 1st byte in the buffer (Win32 uses little endian format)
			UInt32ToBytes(dw_buffer, dtc_setting_control_option_record);
			status = UDSApi.SvcControlDTCSetting_2013(channel, config, out request, UDSApi.uds_svc_param_cdtcs.PUDS_SVC_PARAM_CDTCS_OFF, dtc_setting_control_option_record, 3);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service ResponseOnEvent</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testResponseOnEvent(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] event_type_record = new byte[50];
			byte[] service_to_respond_to_record = new byte[50];

			// Sends a physical ResponseOnEvent message
			event_type_record[0] = 0x08;
			service_to_respond_to_record[0] = (byte)uds_service.PUDS_SERVICE_SI_ReadDTCInformation;
			service_to_respond_to_record[1] = (byte)UDSApi.uds_svc_param_rdtci.PUDS_SVC_PARAM_RDTCI_RNODTCBSM;
			service_to_respond_to_record[2] = 0x01;
			status = UDSApi.SvcResponseOnEvent_2013(channel, config, out request, UDSApi.uds_svc_param_roe.PUDS_SVC_PARAM_ROE_ONDTCS, false, 0x08, event_type_record,
				UDSApi.PUDS_SVC_PARAM_ROE_OTI_LEN, service_to_respond_to_record, 3);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service LinkControl</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testLinkControl(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();

			
			// Sends a physical LinkControl message
			status = UDSApi.SvcLinkControl_2013(channel, config, out request, UDSApi.uds_svc_param_lc.PUDS_SVC_PARAM_LC_VBTWFBR, UDSApi.uds_svc_param_lc_baudrate_identifier.PUDS_SVC_PARAM_LC_BAUDRATE_CAN_500K, 0);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
			// Sends a physical LinkControl message
			status = UDSApi.SvcLinkControl_2013(channel, config, out request, UDSApi.uds_svc_param_lc.PUDS_SVC_PARAM_LC_VBTWSBR, 0, 500000); // 500K = 0x0007a120
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
			// Sends a physical LinkControl message
			status = UDSApi.SvcLinkControl_2013(channel, config, out request, UDSApi.uds_svc_param_lc.PUDS_SVC_PARAM_LC_TB, 0, 0);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service ReadDataByIdentifier</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testReadDataByIdentifier(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			UDSApi.uds_svc_param_di[] buffer = { UDSApi.uds_svc_param_di.PUDS_SVC_PARAM_DI_ADSDID, UDSApi.uds_svc_param_di.PUDS_SVC_PARAM_DI_ECUMDDID };

			
			// Sends a physical ReadDataByIdentifier message
			status = UDSApi.SvcReadDataByIdentifier_2013(channel, config, out request, buffer, 2);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service ReadMemoryByAddress</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testReadMemoryByAddress(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] memory_address_buffer = new byte[10];
			byte[] memory_size_buffer = new byte[10];
			byte memory_address_size = 10;
			byte memory_size_size = 3;

			
			// Sends a physical ReadMemoryByAddress message
			for (int i = 0; i < memory_address_size; i++)
			{
				memory_address_buffer[i] = (byte)('A' + i);
				memory_size_buffer[i] = (byte)('1' + i);
			}
			status = UDSApi.SvcReadMemoryByAddress_2013(channel, config, out request, memory_address_buffer, memory_address_size, memory_size_buffer, memory_size_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service ReadScalingDataByIdentifier</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testReadScalingDataByIdentifier(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();

			
			// Sends a physical ReadScalingDataByIdentifier message
			status = UDSApi.SvcReadScalingDataByIdentifier_2013(channel, config, out request, UDSApi.uds_svc_param_di.PUDS_SVC_PARAM_DI_BSFPDID);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service ReadDataByPeriodicIdentifier</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testReadDataByPeriodicIdentifier(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] periodic_data_identifier = new byte[20];
			ushort periodic_data_identifier_size = 10;

			
			// Sends a physical ReadScalingDataByIdentifier message
			for (int i = 0; i < periodic_data_identifier_size; i++)
				periodic_data_identifier[i] = (byte)('A' + i);
			status = UDSApi.SvcReadDataByPeriodicIdentifier_2013(channel, config, out request, UDSApi.uds_svc_param_rdbpi.PUDS_SVC_PARAM_RDBPI_SAMR, periodic_data_identifier, periodic_data_identifier_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service DynamicallyDefineDataIdentifier</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testDynamicallyDefineDataIdentifier(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			ushort[] source_data_identifier = new ushort[20];
			byte[] memory_size = new byte[20];
			byte[] position_in_source_data_record = new byte[20];
			ushort number_of_elements = 10;
			byte[] memory_address_buffer = new byte[15];
			byte[] memory_size_buffer = new byte[9];
			byte memory_address_size;
			byte memory_size_size;

			
			// Sends a physical DynamicallyDefineDataIdentifierDBID message
			for (int i = 0; i < number_of_elements; i++)
			{
				source_data_identifier[i] = (ushort)(((0xF0 + i) << 8) + ('A' + i));
				memory_size[i] = (byte)(i + 1);
				position_in_source_data_record[i] = (byte)(100 + i);
			}
			status = UDSApi.SvcDynamicallyDefineDataIdentifierDBID_2013(channel, config, out request, UDSApi.uds_svc_param_di.PUDS_SVC_PARAM_DI_CDDID, source_data_identifier,
				memory_size, position_in_source_data_record, number_of_elements);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical UDS_SvcDynamicallyDefineDataIdentifierDBMA message
			number_of_elements = 3;
			memory_address_size = 5;
			memory_size_size = 3;
			for (int j = 0; j < number_of_elements; j++)
			{
				for (int i = 0; i < memory_address_size; i++)
					memory_address_buffer[memory_address_size * j + i] = (byte)((10 * j) + i + 1);
				for (int i = 0; i < memory_size_size; i++)
					memory_size_buffer[memory_size_size * j + i] = (byte)(100 + (10 * j) + i + 1);
			}
			status = UDSApi.SvcDynamicallyDefineDataIdentifierDBMA_2013(channel, config, out request, UDSApi.uds_svc_param_di.PUDS_SVC_PARAM_DI_CESWNDID, memory_address_size,
				memory_size_size, memory_address_buffer, memory_size_buffer, number_of_elements);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical UDS_SvcDynamicallyDefineDataIdentifierCDDDI message
			status = UDSApi.SvcDynamicallyDefineDataIdentifierCDDDI_2013(channel, config, out request, UDSApi.uds_svc_param_di.PUDS_SVC_PARAM_DI_CESWNDID);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical UDS_SvcDynamicallyDefineDataIdentifierClearAllDDDI_2013 message
			status = UDSApi.SvcDynamicallyDefineDataIdentifierClearAllDDDI_2013(channel, config, out request);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service WriteDataByIdentifier</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testWriteDataByIdentifier(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] data_record = new byte[10];
			ushort data_record_size = 10;

			
			// Sends a physical WriteDataByIdentifier message
			for (int i = 0; i < data_record_size; i++)
				data_record[i] = (byte)('A' + i);
			status = UDSApi.SvcWriteDataByIdentifier_2013(channel, config, out request, UDSApi.uds_svc_param_di.PUDS_SVC_PARAM_DI_ASFPDID, data_record, data_record_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service WriteMemoryByAddress</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testWriteMemoryByAddress(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] data_record = new byte[50];
			byte[] memory_address_buffer = new byte[50];
			byte[] memory_size_buffer = new byte[50];
			ushort data_record_size = 50;
			byte memory_address_size = 5;
			byte memory_size_size = 3;

			
			// Sends a physical WriteMemoryByAddress message
			Console.WriteLine(); Console.WriteLine(); Console.WriteLine("Sends a physical WriteMemoryByAddress message: ");
			for (int i = 0; i < data_record_size; i++)
			{
				data_record[i] = (byte)(i + 1);
				memory_address_buffer[i] = (byte)('A' + i);
				memory_size_buffer[i] = (byte)(10 + i);
			}
			status = UDSApi.SvcWriteMemoryByAddress_2013(channel, config, out request, memory_address_buffer, memory_address_size, memory_size_buffer, memory_size_size,
				data_record, data_record_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service ClearDiagnosticInformation</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testClearDiagnosticInformation(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();

			
			// Sends a physical ClearDiagnosticInformation message
			status = UDSApi.SvcClearDiagnosticInformation_2013(channel, config, out request, 0xF1A2B3);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical ClearDiagnosticInformation message with memory selection parameter (2020)
			status = UDSApi.SvcClearDiagnosticInformation_2020(channel, config, out request, 0xF1A2B3, 0x42);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service ReadDTCInformation</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testReadDTCInformation(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();

			
			// Sends a physical ReadDTCInformation message
			status = UDSApi.SvcReadDTCInformation_2013(channel, config, out request, UDSApi.uds_svc_param_rdtci.PUDS_SVC_PARAM_RDTCI_RNODTCBSM, 0xF1);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical ReadDTCInformationRDTCSSBDTC message
			status = UDSApi.SvcReadDTCInformationRDTCSSBDTC_2013(channel, config, out request, 0x00A1B2B3, 0x12);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical ReadDTCInformationRDTCSSBRN message
			status = UDSApi.SvcReadDTCInformationRDTCSSBRN_2013(channel, config, out request, 0x12);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical ReadDTCInformationReportExtended message
			status = UDSApi.SvcReadDTCInformationReportExtended_2013(channel, config, out request, UDSApi.uds_svc_param_rdtci.PUDS_SVC_PARAM_RDTCI_RDTCEDRBDN, 0x00A1B2B3,
				0x12);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical ReadDTCInformationReportSeverity message
			status = UDSApi.SvcReadDTCInformationReportSeverity_2013(channel, config, out request, UDSApi.uds_svc_param_rdtci.PUDS_SVC_PARAM_RDTCI_RNODTCBSMR, 0xF1, 0x12);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical ReadDTCInformationRSIODTC message
			status = UDSApi.SvcReadDTCInformationRSIODTC_2013(channel, config, out request, 0xF1A1B2B3);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical ReadDTCInformationNoParam message
			status = UDSApi.SvcReadDTCInformationNoParam_2013(channel, config, out request, UDSApi.uds_svc_param_rdtci.PUDS_SVC_PARAM_RDTCI_RSUPDTC);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical UDS_SvcReadDTCInformationRDTCEDBR_2013 message
			status = UDSApi.SvcReadDTCInformationRDTCEDBR_2013(channel, config, out request, 0x12);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical UDS_SvcReadDTCInformationRUDMDTCBSM_2013 message
			status = UDSApi.SvcReadDTCInformationRUDMDTCBSM_2013(channel, config, out request, 0x12, 0x34);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical UDS_SvcReadDTCInformationRUDMDTCSSBDTC_2013 message
			status = UDSApi.SvcReadDTCInformationRUDMDTCSSBDTC_2013(channel, config, out request, 0x00123456, 0x78, 0x9A);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical UDS_SvcReadDTCInformationRUDMDTCEDRBDN_2013 message
			status = UDSApi.SvcReadDTCInformationRUDMDTCEDRBDN_2013(channel, config, out request, 0x00123456, 0x78, 0x9A);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical UDS_SvcReadDTCInformationRDTCEDI_2020 message
			status = UDSApi.SvcReadDTCInformationRDTCEDI_2020(channel, config, out request, 0x12);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical UDS_SvcReadDTCInformationRWWHOBDDTCBMR_2013 message
			status = UDSApi.SvcReadDTCInformationRWWHOBDDTCBMR_2013(channel, config, out request, 0x12, 0x34, 0x56);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical UDS_SvcReadDTCInformationRWWHOBDDTCWPS_2013 message
			status = UDSApi.SvcReadDTCInformationRWWHOBDDTCWPS_2013(channel, config, out request, 0x12);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical UDS_SvcReadDTCInformationRDTCBRGI_2020 message
			status = UDSApi.SvcReadDTCInformationRDTCBRGI_2020(channel, config, out request, 0x12, 0x34);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service InputOutputControlByIdentifier</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testInputOutputControlByIdentifier(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] control_option_record = new byte[10];
			byte[] control_enable_mask_record = new byte[10];
			ushort control_option_record_size = 10;
			ushort control_enable_mask_record_size = 5;

			
			// Sends a physical InputOutputControlByIdentifier message
			for (int i = 0; i < control_option_record_size; i++)
			{
				control_option_record[i] = (byte)('A' + i);
				control_enable_mask_record[i] = (byte)(10 + i);
			}
			status = UDSApi.SvcInputOutputControlByIdentifier_2013(channel, config, out request, UDSApi.uds_svc_param_di.PUDS_SVC_PARAM_DI_SSECUSWVNDID, control_option_record,
				control_option_record_size, control_enable_mask_record, control_enable_mask_record_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service RoutineControl</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testRoutineControl(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] routine_control_option_record = new byte[10];
			ushort routine_control_option_record_size = 10;

			
			// Sends a physical RoutineControl message
			for (int i = 0; i < routine_control_option_record_size; i++)
			{
				routine_control_option_record[i] = (byte)('A' + i);
			}
			status = UDSApi.SvcRoutineControl_2013(channel, config, out request, UDSApi.uds_svc_param_rc.PUDS_SVC_PARAM_RC_RRR,
				(UDSApi.uds_svc_param_rc_rid)0xF1A2, routine_control_option_record, routine_control_option_record_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service RequestDownload</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testRequestDownload(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] memory_address_buffer = new byte[15];
			byte[] memory_size_buffer = new byte[15];
			byte memory_address_size = 15;
			byte memory_size_size = 8;

			
			// Sends a physical RequestDownload message
			for (int i = 0; i < memory_address_size; i++)
			{
				memory_address_buffer[i] = (byte)('A' + i);
				memory_size_buffer[i] = (byte)(10 + i);
			}
			status = UDSApi.SvcRequestDownload_2013(channel, config, out request, 0x01, 0x02, memory_address_buffer, memory_address_size, memory_size_buffer,
				memory_size_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service RequestUpload</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testRequestUpload(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] memory_address_buffer = new byte[4];
			byte[] memory_size_buffer = new byte[4];
			byte memory_address_size = 4;
			byte memory_size_size = 4;

			
			// Sends a physical RequestUpload message
			for (int i = 0; i < memory_size_size; i++)
			{
				memory_address_buffer[i] = (byte)('A' + i);
				memory_size_buffer[i] = (byte)(10 + i);
			}
			status = UDSApi.SvcRequestUpload_2013(channel, config, out request, 0x01, 0x02, memory_address_buffer, memory_address_size, memory_size_buffer,
				memory_size_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service RequestTransferExit</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testRequestTransferExit(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] record = new byte[50];
			byte record_size = 20;

			
			// Sends a physical RequestTransferExit message
			for (int i = 0; i < record_size; i++)
			{
				record[i] = (byte)('A' + i);
			}
			status = UDSApi.SvcRequestTransferExit_2013(channel, config, out request, record, record_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service TransferData</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testTransferData(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] record = new byte[50];
			byte record_size = 50;

			
			// Sends a physical TransferData message
			for (int i = 0; i < record_size; i++)
			{
				record[i] = (byte)('A' + i);
			}
			status = UDSApi.SvcTransferData_2013(channel, config, out request, 0x01, record, record_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service TransferData with MAX_DATA length</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testTransferDataBigMessage(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] record = new byte[4095];
			ushort record_size = 4093;

			
			// Sends a physical TransferData message with the maximum data available. The goal is to show that
			// WaitForService_2013 does not return a TIMEOUT error although the transmit and receive time of all the
			// data will be longer than the default time to get a response.
			for (int i = 0; i < record_size; i++)
				record[i] = (byte)('A' + i);
			status = UDSApi.SvcTransferData_2013(channel, config, out request, 0x01, record, record_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}


		/// <summary>Call UDS Service TransferData</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testTransferDataMultipleFunctionalMessage(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg[] tresponse = new uds_msg[1];
			uds_msg confirmation = new uds_msg();
			byte[] record = new byte[5];
			ushort record_size = 5;
			uint response_count = 0;


			// Initialize request message
			config.nai.target_addr = (ushort)uds_address.PUDS_ADDRESS_ISO_15765_4_ADDR_OBD_FUNCTIONAL;
			config.nai.target_type = cantp_isotp_addressing.PCANTP_ISOTP_ADDRESSING_FUNCTIONAL;

			// Sends a functional TransferData message. The goal is to show that UDS_WaitForServiceFunctional_2013 waits long
			// enough to fetch all possible ECU responses.
			for (int i = 0; i < record_size; i++)
			{
				record[i] = (byte)('A' + i);
			}
			status = UDSApi.SvcTransferData_2013(channel, config, out request, 0x01, record, record_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForServiceFunctional_2013(channel, ref request, 1, true, tresponse, out response_count, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref tresponse[0]);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Sample to use event</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testUsingEvent(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_status read_status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uint tmp_event_handle;
			bool res;
			bool stop;

			// set event handler
			System.Threading.AutoResetEvent receive_event = new System.Threading.AutoResetEvent(false);
			tmp_event_handle = Convert.ToUInt32(receive_event.SafeWaitHandle.DangerousGetHandle().ToInt32());
			status = UDSApi.SetValue_2013(cantp_handle.PCANTP_HANDLE_USBBUS1, uds_parameter.PUDS_PARAMETER_RECEIVE_EVENT, ref tmp_event_handle, sizeof(uint));

			if (!UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
			{
				Console.WriteLine("Failed to set event, aborting...");
				receive_event.Close();
				return;
			}

			// Sends a physical TesterPresent message
			status = UDSApi.SvcTesterPresent_2013(channel, config, out request, UDSApi.uds_svc_param_tp.PUDS_SVC_PARAM_TP_ZSUBF);
			
			if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
			{

				// Instead of calling WaitForService function, this sample demonstrates how event can be used. But note that
				// the use of a thread listening to notifications and making the read operations is preferred.
				stop = false;

				// wait until we receive expected response
				do
				{
					res = receive_event.WaitOne(System.Threading.Timeout.Infinite);
					if (res)
					{

						// read all messages
						do
						{
							read_status = UDSApi.Read_2013(channel, out response);
							if (UDSApi.StatusIsOk_2013(read_status, uds_status.PUDS_STATUS_OK, false))
							{
								// this is a simple message check (type and sender/receiver address): to filter UDS request
								// confirmation and get first message from target, but real use-case should check that the UDS
								// service ID matches the request
								if (response.msg.Msgdata_isotp_Copy.netaddrinfo.msgtype == cantp_isotp_msgtype.PCANTP_ISOTP_MSGTYPE_DIAGNOSTIC
									&& response.msg.Msgdata_isotp_Copy.netaddrinfo.source_addr == config.nai.target_addr
									&& response.msg.Msgdata_isotp_Copy.netaddrinfo.target_addr == config.nai.source_addr)
								{
									stop = true;
									
								}
							}

							// Free response message
							status = UDSApi.MsgFree_2013(ref response);
						} while (!UDSApi.StatusIsOk_2013(read_status, uds_status.PUDS_STATUS_NO_MESSAGE));
					}
				} while (!stop);
			}

			// Free request message
			status = UDSApi.MsgFree_2013(ref request);
			
			// Uninitialize event
			tmp_event_handle = 0;
			status = UDSApi.SetValue_2013(channel, uds_parameter.PUDS_PARAMETER_RECEIVE_EVENT, ref tmp_event_handle, sizeof(uint));
			
		}



		/// <summary>Call UDS Service RequestFileTransfer</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testRequestFileTransfer(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] file_size_uncompressed = { 0xD, 0x00 };
			byte[] file_size_compressed = { 0xA, 0x00 };
			byte file_size_length = 2;
			string sfile_name = "toto.txt";
			ushort file_name_length = (ushort)sfile_name.Length;


			
			// Sends a physical RequestFileTransfer message
			status = UDSApi.SvcRequestFileTransfer_2013(channel, config, out request, UDSApi.uds_svc_param_rft_moop.PUDS_SVC_PARAM_RFT_MOOP_REPLFILE, file_name_length,
				sfile_name, 0, 0, file_size_length, file_size_uncompressed, file_size_compressed);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service AccessTimingParameter</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testAccessTimingParameter(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();
			byte[] request_record = { 0xAB, 0xCD };
			uint record_size = 2;


			// Sends a physical AccessTimingParameter message
			status = UDSApi.SvcAccessTimingParameter_2013(channel, config, out request, UDSApi.uds_svc_param_atp.PUDS_SVC_PARAM_ATP_RCATP, request_record, record_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}

		/// <summary>Call UDS Service Authentication</summary>
		/// <param name="channel">cantp channel handle</param>
		/// <param name="config">Configuration of the request message (type, network address information...)</param>
		public void testAuthentication(cantp_handle channel, uds_msgconfig config)
		{
			uds_status status;
			uds_msg request = new Peak.Can.Uds.uds_msg();
			uds_msg response = new uds_msg();
			uds_msg confirmation = new uds_msg();

			byte communication_configuration = 0;
			byte[] certificate_client = new byte[2] { 0x12, 0x34 };
			ushort certificate_client_size = 2;
			byte[] challenge_client = new byte[2] { 0x56, 0x78 };
			ushort challenge_client_size = 2;
			byte[] proof_of_ownership_client = new byte[2] { 0x9A, 0xBC };
			ushort proof_of_ownership_client_size = 2;
			byte[] ephemeral_public_key_client = new byte[2] { 0xDE, 0xF0 };
			ushort ephemeral_public_key_client_size = 2;
			byte[] algorithm_indicator = new byte[16] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
			byte[] additional_parameter = new byte[2] { 0xAA, 0xBB };
			ushort additional_parameter_size = 2;

			
			// Sends a physical Authentication/deAuthenticate message
			status = UDSApi.SvcAuthenticationDA_2020(channel, config, out request);
			if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical Authentication/verifyCertificateUnidirectional message
			status = UDSApi.SvcAuthenticationVCU_2020(channel, config, out request, communication_configuration, certificate_client, certificate_client_size, challenge_client, challenge_client_size);
			if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical Authentication/verifyCertificateBidirectional message
			status = UDSApi.SvcAuthenticationVCB_2020(channel, config, out request, communication_configuration, certificate_client, certificate_client_size, challenge_client, challenge_client_size);
			if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			


			// Sends a physical Authentication/proofOfOwnership message
			status = UDSApi.SvcAuthenticationPOWN_2020(channel, config, out request, proof_of_ownership_client, proof_of_ownership_client_size, ephemeral_public_key_client, ephemeral_public_key_client_size);
			if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical Authentication/requestChallengeForAuthentication message
			status = UDSApi.SvcAuthenticationRCFA_2020(channel, config, out request, communication_configuration, algorithm_indicator);
			if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical Authentication/verifyProofOfOwnershipUnidirectional message
			status = UDSApi.SvcAuthenticationVPOWNU_2020(channel, config, out request, algorithm_indicator, proof_of_ownership_client, proof_of_ownership_client_size, challenge_client, challenge_client_size, additional_parameter, additional_parameter_size);
			if (UDSApi.StatusIsOk_2013(status, uds_status.PUDS_STATUS_OK, false))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical Authentication/verifyProofOfOwnershipBidirectional message
			status = UDSApi.SvcAuthenticationVPOWNB_2020(channel, config, out request, algorithm_indicator, proof_of_ownership_client, proof_of_ownership_client_size, challenge_client, challenge_client_size, additional_parameter, additional_parameter_size);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			

			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			

			// Sends a physical Authentication/authenticationConfiguration message
			status = UDSApi.SvcAuthenticationAC_2020(channel, config, out request);
			if (UDSApi.StatusIsOk_2013(status))
				status = UDSApi.WaitForService_2013(channel, ref request, out response, out confirmation);
			
			// Free messages
			status = UDSApi.MsgFree_2013(ref request);
			status = UDSApi.MsgFree_2013(ref response);
			status = UDSApi.MsgFree_2013(ref confirmation);
			
		}
	}
}
