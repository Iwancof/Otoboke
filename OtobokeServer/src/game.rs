use std::fs;

pub struct Map {
    pub width : usize,
    pub height :usize,
    pub map : Vec<Vec<i32>>
}

impl Map {
    pub fn new(w : usize ,h : usize) -> Map {
        Map{width:w,height:h,map:vec![vec![0;h];w]}
    }

    pub fn create_by_filename(file_name : String) -> Map {
        let result = fs::read_to_string(&file_name);
        let map_data : String;
        match result   {
            Ok(string) =>  {
                map_data = string;
            }
            Err(_) => {
                panic!("[Error]File {} is not exist.",&file_name);
            }
        }
        
        let row : Vec<(usize,String)> = map_data.split('\n').map(|s| s.to_string()).enumerate().collect();
        //dont handle all elements length is not equal. mendo-kusai

        let hei = row.len();
        let wid = row[0].1.len();

        println!("{},{}",hei,wid);

        let mut map_tmp : Vec<Vec<i32>> = vec![vec![0;hei];wid];
        for (i,element) in row {
            //&element[0..(5)].to_string().chars().enumerate().for_each(|(j,nest)| map_tmp[j][i] = nest as i32 - 48);
            element.chars().enumerate().for_each(|(j,nest)| {
                if j < wid  { map_tmp[j][i] = nest as i32 - 48; } 
            });
        }

        Map{width:wid - 1,height:hei,map:map_tmp}
    }
    
    pub fn show_map(&self) {
        for y in 0..self.height {
            for x in 0..self.width {
                print!("{}",self.map[x][y]);
            }
            println!("");
        }
    }

    pub fn map_to_string(&self) -> String {
        let mut ret = String::new();
        for y in 0..self.height {
            for x in 0..self.width {
                ret += &self.map[x][y].to_string();
            }
            ret += ",";
        }
        ret + "|"
    }
}
