using System.Runtime.Intrinsics.Arm;

namespace Movies.Api
{
    public static class ApiEndpoints
    {
     
        public static class V1
        {
            private const string VersionBaseV1 = "api/v1";
            private const string VersionBase = "/api";


            public static class Movies
            {
                private const string Base = $"{VersionBase}/movies";

                public const string Create = Base;
                public const string Get = $"{Base}/{{idOrSlug}}";
                public const string GetAll = Base;
                public const string Update = $"{Base}/{{id}}";
                public const string Delete = $"{Base}/{{id}}";

                public const string Rate = $"{Base}/{{id}}/ratings";
                public const string DeleteRating = $"{Base}/{{id}}/ratings";
            }

            public static class Ratings
            {
                private const string Base = $"{VersionBase}/ratings";

                public const string GetUserRatings = $"{Base}/me";
            }
        }

        public static class V2
        {
            private const string VersionBaseV2 = "api/v2";
            private const string VersionBase = "api";

            public static class Movies
            {
                private const string Base = $"{VersionBase}/movies";

                public const string Create = Base;
                public const string Get = $"{Base}/{{idOrSlug}}";
                public const string GetAll = Base;
                public const string Update = $"{Base}/{{id:guid}}";
                public const string Delete = $"{Base}/{{id:guid}}";

                public const string Rate = $"{Base}/{{id:guid}}/ratings";
                public const string DeleteRating = $"{Base}/{{id:guid}}/ratings";
            }

            public static class Ratings
            {
                private const string Base = $"{VersionBase}/ratings";

                public const string GetUserRatings = $"{Base}/me";
            }
        }
    }
}
