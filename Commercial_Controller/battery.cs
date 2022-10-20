using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Commercial_Controller
{
    public class Battery
    {
        public int ID;
        public string status;
        public List<Column> columnsList;
        public List<FloorRequestButton> floorRequestButtonList;

        public Battery(int _id, int _amountOfColumns, int _amountOfFloors, int _amountOfBasements, int _amountOfElevatorPerColumn)
        {
            this.ID = _id;
            this.status = "online";
            this.columnsList = new List<Column>();
            this.floorRequestButtonList = new List<FloorRequestButton>();
            
            if(_amountOfBasements > 0) {
                this.createBasementRequestButtons(_amountOfFloors);
                this.createBasementColumn(_amountOfBasements, _amountOfElevatorPerColumn);
                _amountOfColumns -= 1;
            }

            this.createFloorRequestButtons(_amountOfFloors);
            this.createColumns(_amountOfColumns, _amountOfFloors, _amountOfElevatorPerColumn);
        }

        public void createBasementColumn(int _amountOfBasements, int _amountOfElevatorPerColumn)
        {
            int columnID = 1;
            List<int> _servedFloorsList = new List<int>();
            int floor = -1;
            for(int i=0; i<_amountOfBasements; i++)
            {
                _servedFloorsList.Add(floor);
                floor -= 1;
            }
            Column column = new Column(columnID, _amountOfBasements, _amountOfElevatorPerColumn, _servedFloorsList, true);
            this.columnsList.Add(column);
            columnID += 1;
            Console.WriteLine("columnid b");
            Console.WriteLine(columnID);
            Console.WriteLine("_nb basement b");
            Console.WriteLine(_amountOfBasements);
            Console.WriteLine("nb elev col b");
            Console.WriteLine(_amountOfElevatorPerColumn);
        }

        public void createColumns(int _amountOfColumns, int _amountOfFloors, int _amountOfElevatorPerColumn)
        {
            int columnID = 1;
            int amountOfFloorsPerColumn = _amountOfFloors / _amountOfColumns;
            int floor = 1;
            List<int> _servedFloorsList = new List<int>();
            for(int j=0; j<_amountOfColumns; j++) {
                for(int i=0; i<_amountOfElevatorPerColumn;i++){
                    if(floor <= _amountOfFloors) {
                        _servedFloorsList.Add(floor);
                        floor += 1;
                    }
                }
            }
            Column column = new Column(columnID, _amountOfFloors, _amountOfElevatorPerColumn, _servedFloorsList, false);
            this.columnsList.Add(column);
            columnID += 1;
            Console.WriteLine("columnid ");
            Console.WriteLine(columnID);
            Console.WriteLine("_nb floor ");
            Console.WriteLine(_amountOfFloors);
            Console.WriteLine("nb elev col ");
            Console.WriteLine(_amountOfElevatorPerColumn);
        }

        public void createFloorRequestButtons(int _amountOfFloors)
        {
            int buttonFloor = 1;
            int floorRequestButtonID = 1;
            for(int i=0; i<_amountOfFloors; i++) {
                FloorRequestButton floorRequestButton = new FloorRequestButton(floorRequestButtonID, buttonFloor, "up");
                this.floorRequestButtonList.Add(floorRequestButton);
                buttonFloor += 1;
                floorRequestButtonID += 1;
            }
        }

        public void createBasementRequestButtons(int _amountOfBasements)
        {
            int buttonFloor = -1;
            int floorRequestButtonID = 1;
            for(int i=0; i<_amountOfBasements; i++) {
                FloorRequestButton floorRequestButton = new FloorRequestButton(floorRequestButtonID, buttonFloor, "down");
                this.floorRequestButtonList.Add(floorRequestButton);
                buttonFloor -= 1;
                floorRequestButtonID += 1;
            }
        }

        public Column findBestColumn(int _requestedFloor)
        {
            foreach (var column in this.columnsList)
            {
                if(column.servedFloorsList.Contains(_requestedFloor))
                {
                    return column;
                }
            }
            return this.columnsList[0];
        }

        //Simulate when a user press a button at the lobby
        public (Column, Elevator) assignElevator(int _requestedFloor, string _direction)
        {
            var column = this.findBestColumn(_requestedFloor);
            var elevator = column.findElevator(1, _direction);
            Console.WriteLine(" elev id best");
            Console.WriteLine(elevator.ID);
            elevator.addNewRequest(1);
            elevator.move();
            elevator.addNewRequest(_requestedFloor);
            elevator.move();
            return (column, elevator);
        }
    }
}