mod server;
mod game;

fn main() { //For one game
    let _g = game::Game::new(3,3);
    
    open_server()
}

fn open_server() {
    let mut g = server::GameController::new();
    println!("Game initialized");

    g.wait_for_players();
    //g.initialize_players(); //distribute map and etc...
    g.start_game();
}

