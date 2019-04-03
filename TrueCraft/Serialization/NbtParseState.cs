﻿namespace TrueCraft.Serialization
{
	internal enum NbtParseState
	{
		AtStreamBeginning,
		AtCompoundBeginning,
		InCompound,
		AtCompoundEnd,
		AtListBeginning,
		InList,
		AtStreamEnd,
		Error
	}
}