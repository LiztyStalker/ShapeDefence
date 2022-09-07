namespace SDefence.Movement
{
    //public enum TYPE_MOVEMENT_ACTION {
    //                                    Move,
    //                                    Curve,
    //                                    Orbit,
    //                                    Spiral,
    //                                    Direct,
    //                                    Wave,
    //                                    Custom,
    //                                }

    public enum TYPE_MOVEMENT_ARRIVE {
                                        Destroy,
                                        Stop,
                                        Return,
                                        Repeat,
                                    }
    public enum TYPE_MOVEMENT_COLLISION {
                                        Destroy,
                                        Penetrate,
                                        Bounce,
                                    }
    public enum TYPE_MOVEMENT_TARGET {
                                        Important,
                                        Entity,
                                        Bullet,
                                        Random,
                                    }

}