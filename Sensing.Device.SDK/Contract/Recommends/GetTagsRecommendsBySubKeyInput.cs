using Sensing.SDK.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Sensing.SDK.Contract
{

    public class FaceTagsRecommendsDto
    {
        public IList<FaceRecommendDto> Recommends { get; set; }
        public IList<FaceTagListDto> Tags { get; set; }
    }

    public class FaceRecommendDto:EntityDto<long>
    {
        public string Type { get; set; }
        public string ThingId { get; set; }
        public string Reason { get; set; }
        public long TagId { get; set; }
        public int? TenantId { get; set; }
    }

    public class FaceTagListDto : EntityDto<long>
    {
        public int? TenantId { get; set; }

        /// <summary>
        /// Face's Tag Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tag's Icon Image Url.
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        /// Face's Gender
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Priority.
        /// </summary>
        public int? Priority { get; set; }
        /// <summary>
        /// Format: 20-25
        /// Seperator: -
        /// </summary>
        public string AgeRange { get; set; }

        /// <summary>
        /// Format: 20-25
        /// Seperator: -
        /// </summary>
        public string HappinessRange { get; set; }

        /// <summary>
        /// face's beauty score range.
        /// </summary>
        public string BeautyScoreRange { get; set; }

        /// <summary>
        ///Example:
        /// {
        /// "attributes": {
        ///   "emotion": {
        ///   "sadness": 0.273,
        ///   "neutral": 0,
        ///   "disgust": 0.005,
        ///   "anger": 0.341,
        ///   "surprise": 99.35,
        ///   "fear": 0.03,
        ///   "happiness": 0
        /// }
        ///}
        /// </summary>
        public string ExtensionData { get; set; }
        public bool IsEnabled { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
    public class GetTagAndRecommendsBySubKeyInput
    {
        [Required]
        public string SubKey { get; set; }
        public string Gender { get; set; }
        public int? Age { get; set; }
        public int? Happiness { get; set; }
        public int? BeautyScore { get; set; }

        public string ExtensionData { get; set; }
    }

    public class GetTagRecommendsByFaceInput
    {
        [Required]
        public string SubKey { get; set; }
        
        public byte[] HeadImage { get; set; }

        public string ExtensionData { get; set; }
    }
}
