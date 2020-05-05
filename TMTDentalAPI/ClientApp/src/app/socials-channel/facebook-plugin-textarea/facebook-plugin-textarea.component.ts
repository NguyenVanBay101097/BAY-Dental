import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-facebook-plugin-textarea',
  templateUrl: './facebook-plugin-textarea.component.html',
  styleUrls: ['./facebook-plugin-textarea.component.css']
})
export class FacebookPluginTextareaComponent implements OnInit {
  @Input()  num_CharLeft: string;
  @Output() content_send = new EventEmitter<string>();

  constructor() { }

  ngOnInit() {
  }

  selectEmoji(event) {
    var icon_emoji = event.emoji.native;   
    this.content_send.emit(icon_emoji);
  }
}
