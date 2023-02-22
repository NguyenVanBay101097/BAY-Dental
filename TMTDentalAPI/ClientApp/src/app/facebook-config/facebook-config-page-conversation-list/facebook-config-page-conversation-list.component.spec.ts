import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookConfigPageConversationListComponent } from './facebook-config-page-conversation-list.component';

describe('FacebookConfigPageConversationListComponent', () => {
  let component: FacebookConfigPageConversationListComponent;
  let fixture: ComponentFixture<FacebookConfigPageConversationListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookConfigPageConversationListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookConfigPageConversationListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
