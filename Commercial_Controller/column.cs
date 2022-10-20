using System;
using System.Collections.Generic;

namespace Commercial_Controller
{
    public class Column
    {
        public int ID;
        public string status;
        public List<int> servedFloorsList;
        public List<Elevator> elevatorsList;
        public List<CallButton> callButtonList;
        // public Array<int> ;
        
        public Column(int _id, int _amountOfFloors, int _amountOfElevators, List<int> _servedFloorsList, bool _isBasement)
        {
            this.ID = _id;
            this.status = "offline";
            // this.amountOfFloors = _amountOfFloors;
            // this.amountOfElevators = _amountOfElevators;
            this.elevatorsList = new List<Elevator>();
            this.callButtonList = new List<CallButton>();
            this.servedFloorsList = _servedFloorsList;
            this.createElevators(_amountOfFloors, _amountOfElevators);
            this.createCallButtons(_amountOfFloors, _isBasement);
        }
        //Create buttons with the floor number depending of isBasement rather it"s true or false
        public void createCallButtons(int _amountOfFloors, bool _isBasement)
        {
            int callButtonID = 1;
            if(_isBasement){
                int buttonFloor = -1;
                for(int i=0; i<_amountOfFloors;i++){
                    CallButton callButton = new CallButton(callButtonID, buttonFloor, "up");
                    this.callButtonList.Add(callButton);
                    buttonFloor -= 1;
                    callButtonID += 1;
                }
            }
            else{
                int buttonFloor = 1;
                for(int i=0; i<_amountOfFloors;i++){
                    CallButton callButton = new CallButton(callButtonID, buttonFloor, "down");
                    this.callButtonList.Add(callButton);
                    buttonFloor += 1;
                    callButtonID += 1;
                }
            }
        }
        //Create as many as elevators we are needed
        public void createElevators(int _amountOfFloors, int _amountOfElevators)
        {
            int elevatorID = 1;
            for(int i=0; i<_amountOfElevators;i++){
                Elevator elevator = new Elevator(elevatorID, _amountOfFloors, 1);
                this.elevatorsList.Add(elevator);
                elevatorID += 1;
                // Console.WriteLine(" elev id");
                // Console.WriteLine(elevatorID);
                // Console.WriteLine("_amount floor");
                // Console.WriteLine(_amountOfFloors);
            }
        }

        //Simulate when a user press a button on a floor to go back to the first floor
        public Elevator requestElevator(int userPosition, string direction)
        {
            var elevator = this.findElevator(userPosition, direction);
            elevator.addNewRequest(userPosition);//inconnu
            elevator.move();
            elevator.addNewRequest(1);
            elevator.move();
            return elevator;
        }

        //We use a score system depending on the current elevators state. Since the bestScore and the referenceGap are 
        //higher values than what could be possibly calculated, the first elevator will always become the default bestElevator, 
        //before being compared with to other elevators. If two elevators get the same score, the nearest one is prioritized. Unlike
        //the classic algorithm, the logic isn"t exactly the same depending on if the request is done in the lobby or on a floor.
        public Elevator findElevator(int _requestedFloor, string _requestedDirection){
            Elevator bestElevator;
            int bestScore = 6;
            int referenceGap = 10000000;
            List<object> bestElevatorInformations = new List<object>();
            if(_requestedFloor == 1)
            {
                foreach(Elevator elevator in this.elevatorsList)
                {
                    //The elevator is at the lobby and already has some requests. It is about to leave but has not yet departed
                    if (elevator.currentFloor == 1 & elevator.status == "stopped")
                    {
                        bestElevatorInformations = this.checkIfElevatorIsBetter(1, elevator, bestScore, referenceGap,  _requestedFloor);
                    }
                    //The elevator is at the lobby and has no requests
                    else if (elevator.currentFloor == 1 & elevator.status == "idle")
                    {
                        bestElevatorInformations = this.checkIfElevatorIsBetter(2, elevator, bestScore, referenceGap, _requestedFloor);
                    }
                    //The elevator is lower than me and is coming up. It means that I"m requesting an elevator to go to a basement, and the elevator is on it"s way to me.
                    else if(elevator.currentFloor > 1 & elevator.direction == "up")
                    {
                        bestElevatorInformations = this.checkIfElevatorIsBetter(3, elevator, bestScore, referenceGap, _requestedFloor);
                    }
                    //The elevator is above me and is coming down. It means that I"m requesting an elevator to go to a floor, and the elevator is on it"s way to me
                    else if(elevator.currentFloor < 1 & elevator.direction == "down"){
                        bestElevatorInformations = this.checkIfElevatorIsBetter(3, elevator, bestScore, referenceGap, _requestedFloor);
                    }
                    //The elevator is not at the first floor, but doesn"t have any request
                    else if(elevator.status == "idle")
                    {
                        bestElevatorInformations = this.checkIfElevatorIsBetter(4, elevator, bestScore, referenceGap, 
                        _requestedFloor);
                    }
                    //The elevator is not available, but still could take the call if nothing better is found
                    else
                    {
                        bestElevatorInformations = this.checkIfElevatorIsBetter(5, elevator, bestScore, referenceGap, _requestedFloor);
                    }
                }
            }
            else 
            {
                foreach(Elevator elevator in this.elevatorsList)
                {
                    //The elevator is at the same level as me, and is about to depart to the first floor
                    if (elevator.currentFloor == _requestedFloor & elevator.status == "stopped" & _requestedDirection == elevator.direction)
                    {
                        bestElevatorInformations = this.checkIfElevatorIsBetter(1, elevator, bestScore, referenceGap,  _requestedFloor);
                    }
                    //The elevator is lower than me and is going up. I"m on a basement, and the elevator can pick me up on it"s way
                    else if (_requestedFloor > elevator.currentFloor  & elevator.direction == "up" & _requestedDirection == "up")
                    {
                        bestElevatorInformations = this.checkIfElevatorIsBetter(2, elevator, bestScore, referenceGap, _requestedFloor);
                    }
                    //The elevator is higher than me and is going down. I"m on a floor, and the elevator can pick me up on it"s way
                    else if(_requestedFloor < elevator.currentFloor  & elevator.direction == "down" & _requestedDirection == "down")
                    {
                        bestElevatorInformations = this.checkIfElevatorIsBetter(2, elevator, bestScore, referenceGap, _requestedFloor);
                    }
                    //The elevator is idle and has no requests
                    else if(elevator.status == "idle"){
                        bestElevatorInformations = this.checkIfElevatorIsBetter(4, elevator, bestScore, referenceGap, _requestedFloor);
                    }
                    //The elevator is not available, but still could take the call if nothing better is found
                    else
                    {
                        bestElevatorInformations = this.checkIfElevatorIsBetter(5, elevator, bestScore, referenceGap, _requestedFloor);
                    }
                }
            }
            bestElevator = (Elevator)bestElevatorInformations[0];
            bestScore = (int)bestElevatorInformations[1];
            referenceGap = (int)bestElevatorInformations[2];
            return bestElevator;
        }

        public List<object> checkIfElevatorIsBetter(int scoreToCheck, Elevator newElevator, int bestScore, int referenceGap, int floor)
        {
            Elevator bestElevator;
            List<object> bestElevatorInformations = new List<object>();
            if(scoreToCheck < bestScore)
            {
                bestScore = scoreToCheck;
                bestElevator = newElevator;
                referenceGap = Math.Abs(newElevator.currentFloor - floor);
                bestElevatorInformations.Add(bestElevator);
                bestElevatorInformations.Add(scoreToCheck);
                bestElevatorInformations.Add(referenceGap);
            }
            else if(bestScore == scoreToCheck)
            {
                var gap = Math.Abs(newElevator.currentFloor - floor);
                if(referenceGap > gap)
                {
                    bestElevator = newElevator;
                    referenceGap = gap;
                    bestElevatorInformations.Add(bestElevator);
                    bestElevatorInformations.Add(scoreToCheck);
                    bestElevatorInformations.Add(referenceGap);
                }
            }
            return bestElevatorInformations;
        }
    }
}