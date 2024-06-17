namespace ViewModel.Extensions
{
    public static class PlayerControllerViewModelExtension
    {
        public static void RequestMove(this PlayerControllerViewModel vm, float x, float y)
        {
            vm.MoveX = x;
            vm.MoveY = y;
        }

        public static void RequestJump(this PlayerControllerViewModel vm)
        {
            if (vm.IsGrounded)
            {
                vm.MoveZ = vm.JumpForce;
                vm.IsGrounded = false;
            }
        }
    }
}
