mod server;
mod game;

fn main() { //For one game
    let map = game::Map::create_by_filename("../maps/default_map".to_string());
    
    //map.show_map();
    //println!("{}",game_instance.map_to_String());

    open_server(map);
}

fn open_server(map : game::Map) {
    let mut g = server::GameController::new();
    println!("Game initialized");

    g.wait_for_players();
    //g.initialize_players(); //distribute map and etc...

    g.start_game(map);
}

