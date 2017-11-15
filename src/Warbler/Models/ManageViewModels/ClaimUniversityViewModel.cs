using System.Collections.Generic;

namespace Warbler.Models.ManageViewModels
{
    public class ClaimUniversityViewModel
    {
        /// <summary>
        ///   Parameterless ctor used by MVC for model binding.
        /// </summary>
        public ClaimUniversityViewModel() {}

        public ClaimUniversityViewModel(List<University> universities)
            => UniversityChoices = universities;

        public ClaimRequest ClaimRequest { get; set; }
        public List<University> UniversityChoices { get; }
    }
}
