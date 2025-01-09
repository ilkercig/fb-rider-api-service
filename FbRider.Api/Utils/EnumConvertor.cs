using FbRider.Api.Domain.Models;

namespace FbRider.Api.Utils
{
    public static class EnumConvertor
    {
        public static ScoringType GetScoringType(string scoringType)
        {
            return scoringType switch
            {
                "head" => ScoringType.HeadToHead,
                "roto" => ScoringType.Rotisserie,
                _ => ScoringType.Unknown 
            };
        }

    }
}
