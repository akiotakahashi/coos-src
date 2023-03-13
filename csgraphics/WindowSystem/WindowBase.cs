using System;
using System.Drawing;

namespace CooS.WindowSystem {

	public abstract class WindowBase {
	
		WindowBase parent;
		Point position;
		Size size;

		protected WindowBase() {
		}

		WindowBase Parent {
			get {
				return this.parent;
			}
			set {
				this.parent = value;
			}
		}

		Point Position {
			get {
				return this.position;
			}
			set {
				this.position = value;
			}
		}

		Size Size {
			get {
				return this.size;
			}
			set {
				this.size = value;
			}
		}

		void MoveTo(int x, int y) {
			this.Invalidate();
		}

		void Resize(int lx, int ly) {
			this.Invalidate();
		}

		void Invalidate() {
		}

		void Update() {
		}

		void Refresh() {
		}

		void Draw() {
		}

	}

}
