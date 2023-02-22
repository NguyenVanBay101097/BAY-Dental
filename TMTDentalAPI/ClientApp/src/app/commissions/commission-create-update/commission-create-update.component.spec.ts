import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommissionCreateUpdateComponent } from './commission-create-update.component';

describe('CommissionCreateUpdateComponent', () => {
  let component: CommissionCreateUpdateComponent;
  let fixture: ComponentFixture<CommissionCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommissionCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommissionCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
