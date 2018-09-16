import { Injectable } from "@angular/core";
import "rxjs/add/operator/map";

@Injectable()
export class TimerService {
  private timeLeft: number = 60;
  interval;

  getNextTime(): string {
    const date = new Date();

    let minutes = date.getMinutes();

    let seconds = date.getSeconds();
    const secondsRemain = (seconds + 10) - 60;
    if (seconds + 10 > 60) {
      minutes += 1;
      seconds = secondsRemain;
    }
    return [date.getHours(), minutes, seconds + 10]
      .map(current => current).join(":");
  }

  getTimer(): any {
    this.interval = setInterval(() => {
      if (this.timeLeft > 0) {
        this.timeLeft--;
      } else {
        this.timeLeft = 60;
      }
    },
      1000);
    return this.interval;
  }
}

