import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BinaryFileInputComponent } from './binary-file-input.component';

describe('BinaryFileInputComponent', () => {
  let component: BinaryFileInputComponent;
  let fixture: ComponentFixture<BinaryFileInputComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BinaryFileInputComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BinaryFileInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
