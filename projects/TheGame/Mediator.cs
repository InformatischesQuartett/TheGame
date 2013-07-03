using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Examples.TheGame
{
    internal class Mediator
    {
        /// <summary>
        /// Gets or sets the UserID.
        /// </summary>
        /// <value>
        /// The UserID.
        /// </value>
        internal int UserID { get; set; }

        /// <summary>
        /// The last assigned objectID.
        /// </summary>
        private int _objectID;

        /// <summary>
        /// Every client is able to spawn 16,500,000 objects.
        /// </summary>
        private const int Range = 16500000;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator"/> class.
        /// </summary>
        internal Mediator()
        {
            UserID = 1;
            _objectID = -1;
        }

        /// <summary>
        /// Gets a new ObjectID which is based on the UserID
        /// </summary>
        /// <returns></returns>
        internal int GetObjectId()
        {
            if (UserID == -1)
                return -1;

            if (_objectID == -1 || _objectID == ((UserID + 1)*Range) - 1)
                return _objectID = UserID*Range;

            return ++_objectID;
        }
    }
}
