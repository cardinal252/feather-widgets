﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Sitefinity.Frontend.Search.Mvc.Models;
using Telerik.Sitefinity.Services.Search.Data;

namespace FeatherWidgets.TestUnit.DummyClasses.SearchResults
{
    /// <summary>
    /// This class creates dummy <see cref="Telerik.Sitefinity.Frontend.Search.Mvc.Models.SearchResultsModel"/>
    /// </summary>
    public class DummySearchResultsModel : SearchResultsModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DummySearchResultsModel"/> class.
        /// </summary>
        /// <param name="languages">The languages.</param>
        public DummySearchResultsModel(CultureInfo[] languages)
            : base(languages)
        {
            this.IsQueryValid = true;
        }

        /// <summary>
        /// Gets or sets the is query valid.
        /// </summary>
        /// <value>
        /// The is query valid.
        /// </value>
        public bool IsQueryValid { get; set; }

        /// <summary>
        /// Validates the query.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <returns></returns>
        public override bool ValidateQuery(ref string searchQuery)
        {
            return this.IsQueryValid;
        }

        /// <summary>
        /// Gets the order list from the model.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public IEnumerable<string> GetModelOrderList()
        {
            return this.GetOrderList();
        }

        /// <summary>
        /// Populates dummy results.
        /// </summary>
        /// <param name="searchQuery">The search query.</param>
        /// <param name="skip">The skip.</param>
        /// <param name="language">The language.</param>
        public override void PopulateResults(string searchQuery, string indexCatalogue, int? skip, string language, string orderBy)
        {
            this.InitializeOrderByEnum(orderBy);

            var totalCount = 3;
            var data = new List<IDocument>();
            this.Results = new ResultModel(data, totalCount);
        }

        /// <summary>
        /// Initializes the order by enum.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Enum.TryParse<Telerik.Sitefinity.Frontend.Search.Mvc.Models.OrderByOptions>(System.String,System.Boolean,Telerik.Sitefinity.Frontend.Search.Mvc.Models.OrderByOptions@)")]
        private void InitializeOrderByEnum(string orderBy)
        {
            if (!orderBy.IsNullOrEmpty())
            {
                OrderByOptions orderByOption;
                Enum.TryParse<OrderByOptions>(orderBy, true, out orderByOption);
                this.OrderBy = orderByOption;
            }
        }
    }
}
