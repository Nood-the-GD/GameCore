namespace RobotCafe
{
    // Marker: a world object that a tap can select. RobotMoveInputView raycasts on tap,
    // skips taps that land on blocking UI, and publishes OnObjectSelected for the hit
    // ISelectable. Consumers still filter by their own component/reference.
    public interface ISelectable { }
}
