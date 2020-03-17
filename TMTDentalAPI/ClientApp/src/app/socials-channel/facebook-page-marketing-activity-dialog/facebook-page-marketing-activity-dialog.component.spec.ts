import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPageMarketingActivityDialogComponent } from './facebook-page-marketing-activity-dialog.component';

describe('FacebookPageMarketingActivityDialogComponent', () => {
  let component: FacebookPageMarketingActivityDialogComponent;
  let fixture: ComponentFixture<FacebookPageMarketingActivityDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPageMarketingActivityDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPageMarketingActivityDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
