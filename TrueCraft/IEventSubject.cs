using System;

namespace TrueCraft
{
	/// <summary>
	///  The subject of an event. When it's disposed, it raises an event and the associated
	///  scheduled events are discarded.
	/// </summary>
	public interface IEventSubject : IDisposable
	{
		event EventHandler Disposed;
	}
}