using System;

namespace CooS.FileSystem.CDFS {

	[Flags]
	public enum FileFlags : byte {
		/// <summary>
		/// If set to ZERO, shall mean that the existence of the file shall be made known to the user upon an inquiry by the user.
		/// If set to ONE, shall mean that the existence of the file need not be made known to the user.
		/// </summary>
		Existence		= 1,
		/// <summary>
		/// If set to ZERO, shall mean that the Directory Record does not identify a directory.
		/// If set to ONE, shall mean that the Directory Record identifies a directory.
		/// </summary>
		Directory		= 2,
		/// <summary>
		/// If set to ZERO, shall mean that the file is not an Associated File.
		/// If set to ONE, shall mean that the file is an Associated File.
		/// </summary>
		AssociatedFile	= 4,
		/// <summary>
		/// If set to ZERO, shall mean that the structure of the information in the file is not
		/// specified by the Record Format field of any associated Extended Attribute Record (see 9.5.8).
		/// If set to ONE, shall mean that the structure of the information in the file has a
		/// record format specified by a number other than zero in the Record Format Field of
		/// the Extended Attribute Record (see 9.5.8).
		/// </summary>
		Record			= 8,
		/// <summary>
		/// If set to ZERO, shall mean that
		///		- an Owner Identification and a Group Identification are not specified for the file (see 9.5.1 and 9.5.2);
		///		- any user may read or execute the file (see 9.5.3).
		/// If set to ONE, shall mean that
		///		- an Owner Identification and a Group Identification are specified for the file (see 9.5.1 and 9.5.2);
		///		- at least one of the even-numbered bits or bit 0 in the Permissions field of the
		/// associated Extended Attribute Record is set to ONE (see 9.5.3).
		/// </summary>
		Protection		= 16,
		/// <summary>
		/// If set to ZERO, shall mean that this is the final Directory Record for the file.
		/// If set to ONE, shall mean that this is not the final Directory Record for the file.
		/// </summary>
		MultiExtent		= 128,
	}

}
