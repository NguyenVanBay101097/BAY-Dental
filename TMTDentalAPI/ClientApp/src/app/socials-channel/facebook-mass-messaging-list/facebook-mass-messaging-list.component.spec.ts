import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookMassMessagingListComponent } from './facebook-mass-messaging-list.component';

describe('FacebookMassMessagingListComponent', () => {
  let component: FacebookMassMessagingListComponent;
  let fixture: ComponentFixture<FacebookMassMessagingListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookMassMessagingListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookMassMessagingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
