import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPageMarketingMessageAddButtonComponent } from './facebook-page-marketing-message-add-button.component';

describe('FacebookPageMarketingMessageAddButtonComponent', () => {
  let component: FacebookPageMarketingMessageAddButtonComponent;
  let fixture: ComponentFixture<FacebookPageMarketingMessageAddButtonComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPageMarketingMessageAddButtonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPageMarketingMessageAddButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
