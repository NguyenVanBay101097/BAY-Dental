import { Component, OnInit } from '@angular/core';
import { FacebookConfigPageService } from '../shared/facebook-config-page.service';

@Component({
  selector: 'app-facebook-config-page-conversations',
  templateUrl: './facebook-config-page-conversations.component.html',
  styleUrls: ['./facebook-config-page-conversations.component.css']
})
export class FacebookConfigPageConversationsComponent implements OnInit {
  selectedPageId = '';
  conversations: any[] = [];
  conversationSelected: any;
  constructor(private configPageService: FacebookConfigPageService) { }

  ngOnInit() {
    this.loadConversations();
  }

  loadConversations() {
    this.configPageService.getConversations('565832646833390', { configId: '3f3f4c42-b177-4a4e-484c-08d7be7aa51f' })
      .subscribe((result: any) => {
        this.conversations = result;
      })
  }

  onConversationChange(conv) {
    this.conversationSelected = conv;
  }

}
