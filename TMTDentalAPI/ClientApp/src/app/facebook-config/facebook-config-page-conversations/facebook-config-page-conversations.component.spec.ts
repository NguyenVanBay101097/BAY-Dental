import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookConfigPageConversationsComponent } from './facebook-config-page-conversations.component';

describe('FacebookConfigPageConversationsComponent', () => {
  let component: FacebookConfigPageConversationsComponent;
  let fixture: ComponentFixture<FacebookConfigPageConversationsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookConfigPageConversationsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookConfigPageConversationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
