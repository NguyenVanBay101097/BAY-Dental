import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-facebook-config-page-conversation-list',
  templateUrl: './facebook-config-page-conversation-list.component.html',
  styleUrls: ['./facebook-config-page-conversation-list.component.css']
})
export class FacebookConfigPageConversationListComponent implements OnInit {
  @Input() conversations: any[] = [];
  @Input() conversationSelected: any[] = [];
  @Output() conversationChange = new EventEmitter<any>();
  constructor() { }

  ngOnInit() {
  }

  selectConversation(conversation: any) {
    this.conversationChange.emit(conversation);
  }
}
