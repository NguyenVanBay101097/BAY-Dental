import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, SimpleChanges } from '@angular/core';
import * as moment from 'moment';

@Component({
  selector: 'app-header-countdown',
  templateUrl: './header-countdown.component.html',
  styleUrls: ['./header-countdown.component.css']
})
export class HeaderCountdownComponent implements OnInit, OnDestroy, OnChanges {

  timeoutId = 0;
  millis = 0;
  interval = 60000;
  @Input() countdown: number = 0; //seconds!

  seconds: number;
  minutes: number;
  hours: number;
  days: number;

  sseconds: string | number;
  mminutes: string | number;
  hhours: string | number;
  ddays: string | number;

  @Output() timerStarted = new EventEmitter<any>();

  constructor() { }
  ngOnChanges(changes: SimpleChanges): void {
    this.start();
  }

  ngOnDestroy(): void {
    this.resetTimeout();
  }

  ngOnInit(): void {
    this.start();
  }

  start() {
    if (this.countdown <= 0) {
      return;
    }

    this.resetTimeout();
    this.tick();
  }

  tick() {
    this.millis = this.countdown * 60 * 1000;
    if (this.millis < 0) {
      this.stop();
      this.millis = 0;
      this.calculateTimeUnits();
      return;
    }

    this.calculateTimeUnits();

    this.timeoutId = window.setTimeout(() => {
      this.tick();
    }, this.interval);

    if (this.countdown > 0) {
      this.countdown--;
    }
    else if (this.countdown <= 0) {
      this.stop();
    }
  }

  stop() {
    this.clear();
  }

  clear() {
    this.resetTimeout();
  }

  resetTimeout() {
    clearTimeout(this.timeoutId);
  }

  calculateTimeUnits() {
    debugger;
    this.seconds = Math.floor((this.millis / 1000) % 60);
    this.minutes = Math.floor(((this.millis / (60000)) % 60));
    this.hours = Math.floor(((this.millis / (3600000)) % 24));
    this.days = Math.floor(((this.millis / (3600000)) / 24));

    this.sseconds = this.seconds < 10 ? '0' + this.seconds : this.seconds;
    this.mminutes = this.minutes < 10 ? '0' + this.minutes : this.minutes;
    this.hhours = this.hours < 10 ? '0' + this.hours : this.hours;
    this.ddays = this.days < 10 ? '0' + this.days : this.days;
  }
}