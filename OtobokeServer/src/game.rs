pub struct Game {
    pub width : i32,
    pub height :i32,
    pub map : Vec<Vec<i32>>
}

impl Game {
    pub fn new(w : i32 ,h : i32) -> Game {
        Game{width:w,height:h,map:vec![vec![0;10];10]}
    }
}
