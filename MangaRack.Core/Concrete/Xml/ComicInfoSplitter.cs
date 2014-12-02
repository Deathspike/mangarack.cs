// ======================================================================
// This source code form is subject to the terms of the Mozilla Public
// License, version 2.0. If a copy of the MPL was not distributed with 
// this file, you can obtain one at http://mozilla.org/MPL/2.0/.
// ======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace MangaRack.Core.Concrete.Xml {
    /// <summary>
    /// Represents a collection of comma separated comic information.
    /// </summary>
    public sealed class ComicInfoSplitter : List<string>, IXmlSerializable {
        #region Constructor
        /// <summary>
        /// Initialize a new instance of the ComicInfoSplitter class.
        /// </summary>
        public ComicInfoSplitter() {}

        /// <summary>
        /// Initialize a new instance of the ComicInfoSplitter class.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public ComicInfoSplitter(IEnumerable<string> collection) {
            foreach (var value in collection) {
                Add(value);
            }
        }
        #endregion

        #region IXmlSerializable
        /// <summary>
        /// This method is reserved and should not be used.
        /// </summary>
        public XmlSchema GetSchema() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="xmlReader">The stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader xmlReader) {
            foreach (var value in xmlReader.ReadElementContentAsString().Split(',')) {
                Add(value.Trim());
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="xmlWriter">The stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter xmlWriter) {
            if (Count != 0) {
                xmlWriter.WriteString(string.Join(", ", this.Where(x => !string.IsNullOrEmpty(x)).ToArray()));
            }
        }
        #endregion
    }
}