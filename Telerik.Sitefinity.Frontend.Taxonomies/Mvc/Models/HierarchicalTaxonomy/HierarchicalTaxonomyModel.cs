﻿using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Sitefinity.Taxonomies.Model;

namespace Telerik.Sitefinity.Frontend.Taxonomies.Mvc.Models.HierarchicalTaxonomy
{
    /// <summary>
    /// The hierarchical taxonomy model.
    /// </summary>
    public class HierarchicalTaxonomyModel : TaxonomyModel, IHierarchicalTaxonomyModel
    {
        #region Properties
        /// <summary>
        /// Determines what taxa will be displayed by the widget.
        /// </summary>
        /// <value>The taxa to display.</value>
        public HierarchicalTaxaToDisplay TaxaToDisplay { get; set; }

        /// <summary>
        /// Determines how many levels from the hierarchy to include.
        /// </summary>
        /// <value>The levels.</value>
        public int Levels { get; set; }

        /// <summary>
        /// Gets or sets the root taxon which children will be displayed as a top level in the widget.
        /// Used only if this display mode is selected.
        /// </summary>
        /// <value>The parent category.</value>
        public Guid RootTaxonId { get; set; }

        /// <summary>
        /// If set to true, all hierarchical taxa will be shown as a flat list.
        /// </summary>
        /// <value>The flatten hierarchy.</value>
        public bool FlattenHierarchy
        {
            get
            {
                return this.flattenHierarchy;
            }
            set
            {
                this.flattenHierarchy = value;
            }
        }
        #endregion

        #region Overriden methods
        /// <summary>
        /// Creates the view model.
        /// </summary>
        /// <returns></returns>
        public override TaxonomyViewModel CreateViewModel()
        {
            var viewModel = new TaxonomyViewModel();
            viewModel.ShowItemCount = this.ShowItemCount;

            if (this.ContentId != Guid.Empty)
            {
                viewModel.Taxa = this.GetTaxaByContentItem();
                return viewModel;
            }

            switch (this.TaxaToDisplay)
            {
                case HierarchicalTaxaToDisplay.All:
                    viewModel.Taxa = this.GetAllTaxa();
                    break;
                case HierarchicalTaxaToDisplay.TopLevel:
                    viewModel.Taxa = this.GetTopLevelTaxa();
                    break;
                case HierarchicalTaxaToDisplay.UnderParticularTaxon:
                    viewModel.Taxa = this.GetTaxaByParent();
                    break;
                case HierarchicalTaxaToDisplay.Selected:
                    viewModel.Taxa = this.GetSpecificTaxa<HierarchicalTaxon>();
                    break;
                case HierarchicalTaxaToDisplay.UsedByContentType:
                    viewModel.Taxa = this.GetTaxaByContentType();
                    break;
                default:
                    viewModel.Taxa = this.GetTopLevelTaxa();
                    break;
            }
            return viewModel;
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Gets view models of all available taxa in a flat list.
        /// </summary>
        /// <returns></returns>
        protected virtual IList<TaxonViewModel> GetAllTaxa()
        {
            var statistics = this.GetTaxonomyStatistics();

            var taxa = this.CurrentTaxonomyManager.GetTaxa<HierarchicalTaxon>()
                .Where(t => t.Taxonomy.Id == this.TaxonomyId);

            return this.GetFlatTaxaViewModelsWithStatistics(taxa, statistics);
        }

        /// <summary>
        /// Gets the taxa view model trees starting from the root level.
        /// </summary>
        /// <returns></returns>
        protected virtual IList<TaxonViewModel> GetTopLevelTaxa()
        {
            var statistics = this.GetTaxonomyStatistics();

            var taxa = this.CurrentTaxonomyManager.GetTaxa<HierarchicalTaxon>()
                .Where(t => t.Taxonomy.Id == this.TaxonomyId && t.Parent == null);

            return GetTaxaViewModels(statistics, taxa);
        }

        /// <summary>
        /// Gets the taxa view model trees starting from the children of the provided parent id.
        /// </summary>
        /// <returns></returns>
        protected virtual IList<TaxonViewModel> GetTaxaByParent()
        {
            var statistics = this.GetTaxonomyStatistics();

            var rootTaxon = this.CurrentTaxonomyManager.GetTaxon(this.RootTaxonId) as HierarchicalTaxon;

            if (rootTaxon != null)
            {
                return this.GetTaxaViewModels(statistics, rootTaxon.Subtaxa);
            }

            return new List<TaxonViewModel>();
        }

        /// <summary>
        /// Creates trees of view models each representing a taxon that is used by the content type that the widget is set to work with.
        /// </summary>
        /// <returns></returns>
        protected virtual IList<TaxonViewModel> GetTaxaByContentType()
        {
            var statistics = this.GetTaxonomyStatistics();

            var contentProviderName = this.GetContentProviderName();

            if (this.ContentType != null)
            {
                statistics = statistics.Where(s => s.DataItemType == this.ContentType.FullName);
            }

            if (!string.IsNullOrWhiteSpace(contentProviderName))
            {
                statistics = statistics.Where(s => s.ItemProviderName == contentProviderName);
            }

            IEnumerable<HierarchicalTaxon> taxa;
            if (this.FlattenHierarchy)
            {
                taxa = this.CurrentTaxonomyManager.GetTaxa<HierarchicalTaxon>()
                    .Where(t => t.Taxonomy.Id == this.TaxonomyId);

            }
            else
            {
                taxa = this.CurrentTaxonomyManager.GetTaxa<HierarchicalTaxon>()
                    .Where(t => t.Taxonomy.Id == this.TaxonomyId && t.Parent == null);
            }

            return this.GetTaxaViewModels(statistics, taxa);
        }

        /// <summary>
        /// Gets the view models of the given taxa.
        /// </summary>
        /// <param name="statistics">The statistics.</param>
        /// <param name="taxa">The taxa.</param>
        /// <returns>Returns flat or hierarchical structure based on the widget settings.</returns>
        protected virtual IList<TaxonViewModel> GetTaxaViewModels(IQueryable<TaxonomyStatistic> statistics, IEnumerable<HierarchicalTaxon> taxa)
        {
            if (this.FlattenHierarchy)
            {
                return this.GetFlatTaxaViewModelsWithStatistics(taxa, statistics);
            }

            return TaxaViewModelTreeBuilder.BuildTaxaTree(
                taxa,
                taxon => this.FilterTaxonByCount(taxon, statistics));
        }
        #endregion

        #region Private fields and constants
        private bool flattenHierarchy = true;
        #endregion
    }
}
