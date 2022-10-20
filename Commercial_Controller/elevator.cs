using System.Threading;
using System.Collections.Generic;
using System.Linq;

namespace Commercial_Controller
{
    public class Elevator
    {
        public int ID;
        public string status;
        public int currentFloor;
        public string direction;
        public bool overweight;
        public int amountOfFloors;
        public List<int> floorRequestsList;
        public List<int> completedRequestsList;
        public Door door;

        public Elevator(int _id, int _amountOfFloors, int _currentFloor)
        {
            this.ID = _id;
            this.status = "online";
            this.amountOfFloors = _amountOfFloors;
            this.currentFloor = _currentFloor;
            this.direction = "";            
            this.overweight = false;
            this.door = new Door(_id);
            this.completedRequestsList = new List<int>();
            this.floorRequestsList = new List<int>();
        }

        public void move()
        {
            while(this.floorRequestsList.Count != 0){
                int destination = this.floorRequestsList[0];
                this.status = "moving";
                if(this.direction == "up"){
                    while(this.currentFloor < destination){
                        this.currentFloor ++;
                    }
                }
                else if(this.direction == "down"){
                    while(this.currentFloor > destination){
                        this.currentFloor --;
                    }
                }
                this.status = "stopped";
                this.operateDoors();
                this.floorRequestsList.RemoveAt(0);
                this.completedRequestsList.Add(destination);
            }
            this.status = "idle";
            this.direction = "";
        }
        
        public void sortFloorList() 
        {
            if(this.direction == "up")
            {
                this.floorRequestsList.Sort();
            }
            else
            {
                this.floorRequestsList.Reverse();
            }
        }

        public void operateDoors()
        {
            bool obstruction = false;
            this.door.status = "opened";
            if(!this.overweight){
                this.door.status = "closing";
                if(!obstruction){
                    this.door.status = "closed";
                }
                else{
                    this.operateDoors();
                }
            }
            else {
                while(this.overweight == false){
                    this.overweight = true;
                }
                this.operateDoors();
            }
        }

        public void addNewRequest(int _requestedFloor)
        {
            if(!this.floorRequestsList.Contains(_requestedFloor)){
                this.floorRequestsList.Add(_requestedFloor);  
            }
            if(this.currentFloor < _requestedFloor){
                this.direction = "up";
            }
            if(this.currentFloor > _requestedFloor){
                this.direction = "down";
            }
        }
    }
}