﻿using System;
using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MyBlog.WebApi.Framework.DTOs.Paging
{
    /// <summary>
    /// Base class for pageable models
    /// </summary>
    public abstract class BasePageableDto : IPageableDto
    {
        #region Methods

        /// <summary>
        /// Ctor
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="pagedList">Entities (models)</param>
        public virtual void LoadPagedList<T>(IPagedList<T> pagedList)
        {
            FirstItem = (pagedList.PageIndex * pagedList.PageSize) + 1;
            HasNextPage = pagedList.HasNextPage;
            HasPreviousPage = pagedList.HasPreviousPage;
            LastItem = Math.Min(pagedList.TotalCount, ((pagedList.PageIndex * pagedList.PageSize) + pagedList.PageSize));
            PageNumber = pagedList.PageIndex + 1;
            PageSize = pagedList.PageSize;
            TotalItems = pagedList.TotalCount;
            TotalPages = pagedList.TotalPages;
            NextPageNumber = HasNextPage ? PageNumber + 1 : TotalPages;
            PreviousPageNumber = HasPreviousPage ? PageNumber - 1 : 1;
        }
        #endregion

        #region Properties

        /// <summary>
        /// The current page index (starts from 0)
        /// </summary>
        public int PageIndex
        {
            get
            {
                if (PageNumber > 0)
                    return PageNumber - 1;

                return 0;
            }
        }

        /// <summary>
        /// The current page number (starts from 1)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The number of items in each page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// The total number of items.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// The total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// The index of the first item in the page.
        /// </summary>
        public int FirstItem { get; set; }

        /// <summary>
        /// The index of the last item in the page.
        /// </summary>
        public int LastItem { get; set; }

        /// <summary>
        /// Whether there are pages before the current page.
        /// </summary>
        public bool HasPreviousPage { get; set; }

        /// <summary>
        /// Whether there are pages after the current page.
        /// </summary>
        public bool HasNextPage { get; set; }

        /// <summary>
        /// If there is a page after it, information about which page it is
        /// </summary>
        public int NextPageNumber { get; set; }

        /// <summary>
        /// If there is a page before it, information about which page it is
        /// </summary>
        public int PreviousPageNumber { get; set; }

        #endregion
    }
}