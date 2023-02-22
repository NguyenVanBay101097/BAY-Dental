import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LaboBridgeListComponent } from './labo-bridge-list.component';

describe('LaboBridgeListComponent', () => {
  let component: LaboBridgeListComponent;
  let fixture: ComponentFixture<LaboBridgeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LaboBridgeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LaboBridgeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
