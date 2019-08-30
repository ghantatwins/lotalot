using System;
using System.Collections.Generic;

namespace LottoData.Lib.Interfaces.DataTypes
{
    public interface IDraw:IData,IEnumerable<IData>
    {
        
        DateTime DrawDate { get; }
        /// <summary>
        /// index starts from zero to Maximum number of balls drawn-1, returns the Ball value
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int GetBallByIndex(int index);
        /// <summary>
        /// Gets all the balls in array
        /// </summary>
        int[] BallsArray { get; }

        /// <summary>
        /// Gets the sum of all balls
        /// </summary>
        /// <returns></returns>
        long BallsSum { get; }
    
        /// <summary>
        /// Physical position is always index+1
        /// </summary>
        /// <returns></returns>
        long PhysicalPosition { get; }
        /// <summary>
        /// This is memory position in total combinations(0-> Ncr-1)
        /// </summary>
        /// <returns></returns>
        long ModelIndex { get; }
        /// <summary>
        /// return 1/Physical Position
        /// </summary>
        /// <returns></returns>
        decimal Proportion { get; }
        /// <summary>
        /// Adds draw to another Draw
        /// </summary>
        /// <param name="draw"></param>
        /// <returns></returns>
        IDraw Add(IDraw draw);
        /// <summary>
        /// Subtracts draw from another draw
        /// </summary>
        /// <param name="draw"></param>
        /// <returns></returns>
        IDraw Subtract(IDraw draw);
        /// <summary>
        /// multiplies draw by another draw
        /// </summary>
        /// <param name="draw"></param>
        /// <returns></returns>
        IDraw Multiply(IDraw draw);
        /// <summary>
        /// Divides draw by another draw
        /// </summary>
        /// <param name="draw"></param>
        /// <returns></returns>
        IDraw Divide(IDraw draw);

        /// <summary>
        /// Adds scalar value to Draw
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IDraw Add(double value);
        /// <summary>
        /// Subtracts scalar from draw
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IDraw Subtract(double value);
        /// <summary>
        /// multiplies scalar to draw
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IDraw Multiply(double value);
        /// <summary>
        /// Divides scalar to draw
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        IDraw Divide(double value);

        int SumDigitsSum { get; }
        int IndexDigitsSum { get; }
    }
    


}