import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionCreateUpdateDialogComponent } from './commission-create-update-dialog.component';

describe('CommissionCreateUpdateDialogComponent', () => {
  let component: CommissionCreateUpdateDialogComponent;
  let fixture: ComponentFixture<CommissionCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
