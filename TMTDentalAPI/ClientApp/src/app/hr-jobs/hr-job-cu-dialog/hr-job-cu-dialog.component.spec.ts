import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { HrJobCuDialogComponent } from './hr-job-cu-dialog.component';

describe('HrJobCuDialogComponent', () => {
  let component: HrJobCuDialogComponent;
  let fixture: ComponentFixture<HrJobCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ HrJobCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(HrJobCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
