import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MedicineOrderCreateDialogComponent } from './medicine-order-create-dialog.component';

describe('MedicineOrderCreateDialogComponent', () => {
  let component: MedicineOrderCreateDialogComponent;
  let fixture: ComponentFixture<MedicineOrderCreateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MedicineOrderCreateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MedicineOrderCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
