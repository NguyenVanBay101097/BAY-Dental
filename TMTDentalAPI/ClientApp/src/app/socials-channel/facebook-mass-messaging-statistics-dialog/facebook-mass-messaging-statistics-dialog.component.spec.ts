import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookMassMessagingStatisticsDialogComponent } from './facebook-mass-messaging-statistics-dialog.component';

describe('FacebookMassMessagingStatisticsDialogComponent', () => {
  let component: FacebookMassMessagingStatisticsDialogComponent;
  let fixture: ComponentFixture<FacebookMassMessagingStatisticsDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookMassMessagingStatisticsDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookMassMessagingStatisticsDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
