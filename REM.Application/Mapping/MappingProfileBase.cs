using AutoMapper;
using REM.Application.Helper;

namespace REM.Application.Mapping;

public class MappingProfileBase : Profile
{
    protected bool IsArabic => CultureHelper.CurrentLanguage == "ar";

    public MappingProfileBase()
    {
        SourceMemberNamingConvention = new LowerUnderscoreNamingConvention();
        DestinationMemberNamingConvention = new PascalCaseNamingConvention();
        ReplaceMemberName("_", "");
    }
}
