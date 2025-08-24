import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'customCountDown'
})
export class CustomCountDownPipe implements PipeTransform {

  transform(value: number): string {
    
    let secondsS = '';
    let minutesS= "";
    let hoursS = '';
    let seconds = Math.floor((value ) % 60);
    
    if(seconds.toString().length < 2){
      
     secondsS = '0' + seconds.toString();
    }else{
      secondsS = seconds.toString();
    }
    let minutes = Math.floor((value /60 ) % 60);
    if(minutes.toString().length < 2){
      minutesS = '0' + minutes.toString();
    }else{
      minutesS = minutes.toString();
    }
    let hours = Math.floor((value / 3600) );
    if(hours.toString().length < 2){
      hoursS = '0' + hours.toString();
    }else{
      hoursS = hours.toString()
    }

    return ` ${hoursS}:${minutesS}:${secondsS} `;
  }

}
