using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace StoryMakerV3.Models
{
    public class Story
    {
        [Key]
        public int StoryId { get; set; }

        [DisplayName("Story Name")]
        [StringLength(25, MinimumLength = 5)]
        [Required(ErrorMessage = "Story Name is Required")]
        public string StoryName { get; set; }

        [DisplayName("Story Description")]
        public string StoryDescription { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayName("Creation Date")]
        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }

        public String StoryPath { get; set; }
        public bool isArchieved { get; set; }
        public virtual ICollection<Collage> ListOfCollages { get; set; }
    }


    public class Collage
    {
        [Key]
        public int CollegeId { get; set; }

        [DisplayName("Collage Name")]
        [Required]
        [StringLength(25, MinimumLength = 5)]
        public string CollegeName { get; set; }

        [DisplayName("College Description")]
        public string CollegeDescription { get; set; }

        public int StoryId { get; set; }

        [DisplayName("Story Name")]
        public virtual Story StoryName { get; set; }

        [DisplayName("Creation Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime CreationDate { get; set; }

        [DisplayName("Sequence Number")]
        [Required]
        public int SequenceNum { get; set; }


        public string CollagePath { get; set; }

        public bool isArchieved { get; set; }

        public virtual ICollection<StoryBlock> ListOfStoryBlocks { get; set; }
    }

    public class StoryBlock
    {
        [Key]
        public int BlockId { get; set; }

        [DisplayName("Story Block Name")]

        [Required]
        [StringLength(25, MinimumLength = 5)]
        public string BlockName { get; set; }

        public string Caption { get; set; }

        [DisplayName("Block Description")]
        public string Description { get; set; }

        public int BlockSeq { get; set; }

        public bool isArchieved { get; set; }

        [DisplayName("Image Path")]
        public string Imagepath { get; set; }

        public int CollegeId { get; set; }

        [DisplayName("College Name")]
        public virtual Collage Collage { get; set; }

        [DisplayName("Upload Time")]
        public DateTime UploadTime { get; set; }
    }
}