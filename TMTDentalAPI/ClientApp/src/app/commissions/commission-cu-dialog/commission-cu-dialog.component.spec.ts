import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionCuDialogComponent } from './commission-cu-dialog.component';

describe('CommissionCuDialogComponent', () => {
  let component: CommissionCuDialogComponent;
  let fixture: ComponentFixture<CommissionCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
