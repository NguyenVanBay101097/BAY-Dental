import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { NgbPopover } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-tcare-scenario-message-textarea',
  templateUrl: './tcare-scenario-message-textarea.component.html',
  styleUrls: ['./tcare-scenario-message-textarea.component.css']
})
export class TcareScenarioMessageTextareaComponent implements OnInit {

  @ViewChild('popOver', { static: true }) popOver: NgbPopover;
  @Input()  num_CharLeft: string;
  @Output() content_send = new EventEmitter<string>();

  constructor() { }

  ngOnInit() {
  }

  selectEmoji(event) {
    var icon_emoji = event.emoji.native;   
    this.content_send.emit(icon_emoji);
  }

  toggleWithGreeting(popover) {         
    if (popover.isOpen()) {       
      popover.close();
    } else { 
      popover.open();
    }
  }

}
