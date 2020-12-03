using System;

namespace Scribe.Interpreter.BluePrints.Attributes
{
    public class KeywordAttribute: Attribute
    {
        /// <summary>
        /// Gets or sets the keyword used by the managing class.
        /// </summary>
        public string[] Words { get; set; }
    }
}