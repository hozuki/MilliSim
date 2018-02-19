namespace OpenMLTD.MilliSim.Extension.Components.CoreComponents {
    public enum BackgroundImageFit {

        /// <summary>
        /// Do not scale the image. The image is drawn from top left.
        /// </summary>
        None = 0,
        /// <summary>
        /// Center the image, and try to fit the smaller one of width or height constraint.
        /// The image will be scaled an parts outside the screen will be dropped.
        /// </summary>
        Fit = 1,
        /// <summary>
        /// Stretch the image to fit the screen.
        /// </summary>
        Stretch = 2,
        /// <summary>
        /// Tile the image. The image is not scaled and is drawn from top left.
        /// </summary>
        Tile = 3,
        /// <summary>
        /// Do not scale the image but center it.
        /// </summary>
        Center = 4,
        /// <summary>
        /// Center the image, and try to fit the larger one of width or height constraint.
        /// The image will be scaled and transparent-black sides will be added.
        /// </summary>
        LetterBox = 5

    }
}
