﻿using System.ComponentModel;

namespace PocAspNetCoreBlazorAntDesign.Models
{
    public enum ErrorTypeEnum : byte
    {
        [Description("invalid_request")]
        InvalidRequest,

        [Description("unauthorized_client")]
        UnAuthoriazedClient,

        [Description("access_denied")]
        AccessDenied,

        [Description("unsupported_response_type")]
        UnSupportedResponseType,

        [Description("invalid_scope")]
        InValidScope,

        [Description("server_error")]
        ServerError,

        [Description("temporarily_unavailable")]
        TemporarilyUnAvailable,

        [Description("invalid_grant")]
        InvalidGrant,

        [Description("invalid_client")]
        InvalidClient
    }
}
