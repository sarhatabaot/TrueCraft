using Microsoft.Xna.Framework.Graphics;

namespace VisualTests
{
	public interface IVisualTest
	{
		string Name { get; }
		void Update(VisualTestGame game);
		void Draw(SpriteBatch sb, VisualTestGame game);
	}
}